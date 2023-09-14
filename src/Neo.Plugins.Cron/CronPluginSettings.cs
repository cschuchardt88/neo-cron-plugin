// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Microsoft.Extensions.Configuration;

namespace Neo.Plugins.Cron;

public class CronPluginSettings
{
    public uint Network { get; private init; }
    public long MaxGasInvoke { get; private init; }
    public string JobsPath { get; private init; }

    public static CronPluginSettings Current { get; private set; }

    public static CronPluginSettings Default =>
        new()
        {
            Network = 860833102u,
            MaxGasInvoke = 20000000,
            JobsPath = "jobs",
        };

    public static void Load(IConfigurationSection section) =>
        Current = new()
        {
            Network = section.GetValue(nameof(Network), Default.Network),
            MaxGasInvoke = section.GetValue(nameof(MaxGasInvoke), Default.MaxGasInvoke),
            JobsPath = section.GetValue(nameof(JobsPath), Default.JobsPath),
        };
}
