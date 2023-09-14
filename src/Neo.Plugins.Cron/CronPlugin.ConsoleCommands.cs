// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.ConsoleService;

namespace Neo.Plugins.Cron;
public partial class CronPlugin
{
    [ConsoleCommand("crontab jobs list", Category = "Crontab Commands", Description = "List all the crontab jobs.")]
    private void OnListCrontabJobs()
    {
        if (CronPluginSettings.Current.Jobs.Any() == true)
            ConsoleHelper.Info("---------", "Jobs", "---------");

        foreach (var job in CronPluginSettings.Current.Jobs)
        {
            ConsoleHelper.Info("      Name: ", $"\"{job.Name}\"");
            ConsoleHelper.Info("Expression: ", $"{job.Expression}");
            ConsoleHelper.Info("   RunOnce: ", $"{job.RunOnce}");
            ConsoleHelper.Info("", "-------", "Contract", "-------");
            ConsoleHelper.Info("ScriptHash: ", $"{job.Contract.ScriptHash}");
            ConsoleHelper.Info("    Method: ", $"{job.Contract.Method}");
            ConsoleHelper.Info("Parameters: ", $"[{string.Join(", ", job.Contract.Params.Select(s => $"\"{s.Value}\""))}]");
            ConsoleHelper.Info("", "--------", "Wallet", "--------");
            ConsoleHelper.Info("      Path: ", $"\"{job.Wallet.Path}\"");
            ConsoleHelper.Info("   Account: ", $"{job.Wallet.Account}");

            if (CronPluginSettings.Current.Jobs.Length > 1)
                ConsoleHelper.Info();
        }

        if (CronPluginSettings.Current.Jobs.Any() == true)
            ConsoleHelper.Info("----------------------");

        ConsoleHelper.Info("  Total: ", $"{CronPluginSettings.Current.Jobs.Length}", " job(s).");
    }
}
