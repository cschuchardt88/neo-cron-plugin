// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using NCrontab;
using System.Collections.Concurrent;

namespace Neo.Plugins.Cron.Jobs;

internal class CronScheduler : IDisposable
{
    public ConcurrentDictionary<Guid, CronEntry> Entries { get; }

    private readonly ConcurrentDictionary<DateTime, List<ICronJob>> _tasks;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cancelWaitTask;

    public CronScheduler()
    {
        _tasks = new();
        Entries = new();
        _cancelWaitTask = new();
        _timer = new(TimeSpan.FromSeconds(30));
        _ = Task.Run(async () => await WaitForTimer(_cancelWaitTask.Token));
    }

    public void Dispose()
    {
        _cancelWaitTask.Cancel();
        _timer.Dispose();
    }

    public bool TryAdd(CronEntry entry, out Guid entryId)
    {
        entryId = Guid.NewGuid();
        if (Entries.TryAdd(entryId, entry))
            return true;
        entryId = default;
        return false;
    }

    private static DateTime Precision()
    {
        var now = DateTime.UtcNow;
        return new(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    }

    private async Task WaitForTimer(CancellationToken token)
    {
        while (await _timer.WaitForNextTickAsync(token))
        {
            var now = Precision();

            if (_tasks.TryGetValue(now, out var jobs) == true)
                _ = jobs.Select(s => Task.Run(async () => await s.Run(token)));
            Entries.Values.ToList().ForEach(GetDateTimeOccurrences);
        }
    }

    private void GetDateTimeOccurrences(CronEntry entry)
    {
        var now = DateTime.UtcNow;
        foreach (var occurrence in entry.Schedule.GetNextOccurrences(now, now.AddMinutes(1)))
        {
            if (_tasks.TryGetValue(occurrence, out var jobs) == true)
                jobs.Add(entry.Job);
            else
                _tasks[occurrence] = new() { entry.Job };
        }
    }
}

internal class CronEntry
{
    public ICronJob Job { get; }
    public CrontabSchedule Schedule { get; }

    internal CronEntry(
        CrontabSchedule schedule,
        ICronJob job)
    {
        Schedule = schedule;
        Job = job;
    }
}