// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.ConsoleService;
using Neo.Plugins.Crontab.Jobs;

namespace Neo.Plugins.Crontab;
public partial class CronPlugin
{
    [ConsoleCommand("crontab jobs list", Category = "Crontab Commands", Description = "List all the crontab jobs.")]
    private void OnListCrontabJobs()
    {
        if (_scheduler.Entries.Any() == true)
            ConsoleHelper.Info("---------", "Jobs", "---------");

        foreach (var entry in _scheduler.Entries)
        {
            ConsoleHelper.Info("        ID: ", $"{entry.Key:n}");
            ConsoleHelper.Info("      Name: ", $"\"{entry.Value.Settings.Name}\"");
            ConsoleHelper.Info("Expression: ", $"{entry.Value.Settings.Expression}");
            ConsoleHelper.Info("   RunOnce: ", $"{entry.Value.Settings.RunOnce}");
            if (entry.Value.Job.LastRunTimestamp != default && entry.Value.Job.LastRunTimestamp > CronScheduler.PrecisionMinute())
                ConsoleHelper.Info("   RunNext: ", $"{entry.Value.Schedule.GetNextOccurrence(DateTime.Now):MM/dd/yyyy hh:mm tt}");
            else
            {
                if (entry.Value.Job.LastRunTimestamp != default)
                    ConsoleHelper.Info("   RunLast: ", $"{entry.Value.Job.LastRunTimestamp.ToLocalTime():MM/dd/yyyy hh:mm tt}");
                else
                    ConsoleHelper.Info("   RunLast: ", $"Processing...");
            }

            ConsoleHelper.Info("  Filename: ", $"\"{entry.Value.Settings.Filename}\"");
            if (entry.Value.Settings.GetType() == typeof(CronJobBasicSettings))
            {
                var contractSettings = entry.Value.Settings as CronJobBasicSettings;
                ConsoleHelper.Info("", "-------", "Contract", "-------");
                ConsoleHelper.Info("ScriptHash: ", $"{contractSettings.Contract.ScriptHash}");
                ConsoleHelper.Info("    Method: ", $"{contractSettings.Contract.Method}");
                ConsoleHelper.Info("Parameters: ", $"[{string.Join(", ", contractSettings.Contract.Params.Select(s => $"\"{s.Value}\""))}]");
            }
            else if (entry.Value.Settings.GetType() == typeof(CronJobTransferSettings))
            {
                var transferSettings = entry.Value.Settings as CronJobTransferSettings;
                ConsoleHelper.Info("", "-------", "Transfer", "-------");
                ConsoleHelper.Info("   AssetId: ", $"{transferSettings.Transfer.AssetId}");
                ConsoleHelper.Info("        To: ", $"{transferSettings.Transfer.SendTo}");
                ConsoleHelper.Info("    Amount: ", $"{transferSettings.Transfer.SendAmount}");
                ConsoleHelper.Info("   Signers: ", $"[{string.Join(", ", transferSettings.Transfer.Signers.Select(s => $"\"{s}\""))})]");
                ConsoleHelper.Info("      Data: ", $"\"{transferSettings.Transfer.Comment}\"");
            }
            ConsoleHelper.Info("", "--------", "Wallet", "--------");
            ConsoleHelper.Info("      Path: ", $"\"{entry.Value.Settings.Wallet.Path}\"");
            ConsoleHelper.Info("   Account: ", $"{entry.Value.Settings.Wallet.Account}");

            if (_scheduler.Entries.Count > 1)
                ConsoleHelper.Info();
        }

        if (_scheduler.Entries.Any() == true)
            ConsoleHelper.Info("----------------------");

        ConsoleHelper.Info("  Total: ", $"{_scheduler.Entries.Count}", " job(s).");
    }
}
