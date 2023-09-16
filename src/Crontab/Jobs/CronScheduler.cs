// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using System.Collections.Concurrent;

namespace Neo.Plugins.Crontab.Jobs;

internal class CronScheduler : IDisposable
{
    public IReadOnlyDictionary<Guid, CronEntry> Entries => _entries;

    private readonly ConcurrentDictionary<Guid, CronEntry> _entries;
    private readonly ConcurrentDictionary<DateTime, List<ICronJob>> _tasks;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cancelWaitTask;

    public CronScheduler()
    {
        _tasks = new();
        _entries = new();
        _cancelWaitTask = new();
        _timer = new(TimeSpan.FromSeconds(1));
        _ = Task.Run(async () => await WaitForTimer(_cancelWaitTask.Token));
    }

    public void Dispose()
    {
        _cancelWaitTask.Cancel();
        _timer.Dispose();
        _cancelWaitTask.Dispose();
    }

    public bool TryAdd(CronEntry entry, out Guid entryId)
    {
        entryId = Guid.NewGuid();
        if (_entries.TryAdd(entryId, entry))
            return true;
        entryId = default;
        return false;
    }

    public bool TryRemove(Guid entryId, out CronEntry jobEntry)
    {
        if (_entries.TryRemove(entryId, out jobEntry) == false)
            return false;
        else
        {
            var tmpEntry = jobEntry;
            foreach (var allJobTaskList in _tasks.Values)
                allJobTaskList
                    .FindAll(w => ReferenceEquals(tmpEntry.Job, w))
                    .ForEach(f => allJobTaskList.Remove(f));
            return true;
        }
    }

    public bool ContainsTask(ICronJob job) =>
        _tasks.Values.SelectMany(sm => sm).Contains(job);

    internal static DateTime PrecisionMinute()
    {
        var now = DateTime.UtcNow;
        return new(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    }

    private async Task WaitForTimer(CancellationToken token)
    {
        DateTime lastRun = default;
        while (await _timer.WaitForNextTickAsync(token) && token.IsCancellationRequested == false)
        {
            var now = PrecisionMinute();

            if (lastRun == now)
                continue;

            _entries.Values.ToList().ForEach(LoadDateTimeOccurrences);

            if (_tasks.TryGetValue(now, out var jobs) == true)
            {
                using var cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(CronPluginSettings.Current.Job.Timeout));
                await Task.WhenAll(jobs.Select(s => Task.Run(() => s.Run(now), cancelTokenSource.Token))).ConfigureAwait(false);
                _tasks.TryRemove(now, out _);
            }
            lastRun = now;
        }
    }

    private void LoadDateTimeOccurrences(CronEntry entry)
    {
        if (entry.IsEnabled == false)
            return;

        var now = DateTime.UtcNow;
        foreach (var occurrence in entry.Schedule.GetNextOccurrences(now, now.AddMinutes(1)))
        {
            if (_tasks.TryGetValue(occurrence, out var jobs) == false)
            {
                _tasks[occurrence] = new() { entry.Job };
                DisableEntryAfterRunOnce(entry);
            }
            else
            {
                if (jobs.SingleOrDefault(s => ReferenceEquals(s, entry.Job)) == null)
                {
                    jobs.Add(entry.Job);
                    DisableEntryAfterRunOnce(entry);
                }
            }
        }
    }

    private static void DisableEntryAfterRunOnce(CronEntry entry)
    {
        if (entry.Settings.RunOnce == true)
            entry.IsEnabled = false;
    }
}
