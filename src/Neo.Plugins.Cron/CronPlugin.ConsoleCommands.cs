// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.ConsoleService;
using Neo.Plugins.Cron.Jobs;

namespace Neo.Plugins.Cron;
public partial class CronPlugin
{
    [ConsoleCommand("crontab jobs list", Category = "Crontab Commands", Description = "List all the crontab jobs.")]
    private void OnListCrontabJobs()
    {
        if (_scheduler.Entries.Any() == true)
            ConsoleHelper.Info("---------", "Jobs", "---------");

        foreach (var job in _scheduler.Entries)
        {
            ConsoleHelper.Info("        ID: ", $"{job.Key:n}");
            ConsoleHelper.Info("      Name: ", $"\"{job.Value.Settings.Name}\"");
            ConsoleHelper.Info("Expression: ", $"{job.Value.Settings.Expression}");
            ConsoleHelper.Info("   RunOnce: ", $"{job.Value.Settings.RunOnce}");
            if (job.Value.LastRunTime > CronScheduler.PrecisionMinute())
                ConsoleHelper.Info("   RunNext: ", $"{job.Value.Schedule.GetNextOccurrence(DateTime.Now):MM/dd/yyyy hh:mm tt}");
            else
                ConsoleHelper.Info("   RunLast: ", $"{job.Value.LastRunTime.ToLocalTime():MM/dd/yyyy hh:mm tt}");
            ConsoleHelper.Info("", "-------", "Contract", "-------");
            ConsoleHelper.Info("ScriptHash: ", $"{job.Value.Settings.Contract.ScriptHash}");
            ConsoleHelper.Info("    Method: ", $"{job.Value.Settings.Contract.Method}");
            ConsoleHelper.Info("Parameters: ", $"[{string.Join(", ", job.Value.Settings.Contract.Params.Select(s => $"\"{s.Value}\""))}]");
            ConsoleHelper.Info("", "--------", "Wallet", "--------");
            ConsoleHelper.Info("      Path: ", $"\"{job.Value.Settings.Wallet.Path}\"");
            ConsoleHelper.Info("   Account: ", $"{job.Value.Settings.Wallet.Account}");

            if (_scheduler.Entries.Count > 1)
                ConsoleHelper.Info();
        }

        if (_scheduler.Entries.Any() == true)
            ConsoleHelper.Info("----------------------");

        ConsoleHelper.Info("  Total: ", $"{_scheduler.Entries.Count}", " job(s).");
    }
}
