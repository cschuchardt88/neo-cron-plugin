// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using NCrontab;
using Neo.ConsoleService;
using Neo.Plugins.Cron.Jobs;

namespace Neo.Plugins.Cron;

public partial class CronPlugin
{
    private void CreateJob(CronJobSettings settings)
    {
        try
        {
            if (File.Exists(settings.Wallet.Path) == false)
                ConsoleHelper.Error($"Cron:Job[\"{settings.Name}\"]::\"{settings.Wallet.Path} does not exist.\"");
            if (string.IsNullOrEmpty(settings.Contract?.Invoke))
                ConsoleHelper.Error($"Cron:Job[\"{settings.Name}\"]::\"Method name is invalid.\"");
            else
            {
                settings.Contract.Invoke = settings.Contract.Invoke.Length > 1 ?
                    settings.Contract.Invoke[0].ToString().ToLowerInvariant() + settings.Contract.Invoke[1..] :
                    settings.Contract.Invoke[0].ToString().ToLowerInvariant();
                var cTask = CronTask.Create(settings);
                if (cTask.Wallet == null)
                    ConsoleHelper.Error($"Cron:Job[\"{settings.Name}\"]::\"Invalid password.\"");
                else
                {
                    var taskSchedule = CrontabSchedule.TryParse(settings.Expression);
                    if (taskSchedule != null)
                        _ = _scheduler.TryAdd(new CronEntry(taskSchedule, cTask), out _);
                    else
                        ConsoleHelper.Error($"Cron:Job:[\"{settings.Name}\"]::\"Expression is invalid.\"");
                }
            }
        }
        catch (FormatException)
        {
            ConsoleHelper.Error($"Cron:Job:[\"{settings.Name}\"]::\"Invalid address format.\"");
        }
    }
}
