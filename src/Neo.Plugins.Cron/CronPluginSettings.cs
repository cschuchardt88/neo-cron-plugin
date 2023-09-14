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
    public CronJobSettings[] Jobs { get; private set; }

    public static CronPluginSettings Current { get; private set; }

    public static CronPluginSettings Default =>
        new()
        {
            Network = 860833102u,
            MaxGasInvoke = 20000000,
            Jobs = Array.Empty<CronJobSettings>(),
        };

    public static void Load(IConfigurationSection section)
    {
        Current = new()
        {
            Network = section.GetValue(nameof(Network), Default.Network),
            MaxGasInvoke = section.GetValue(nameof(MaxGasInvoke), Default.MaxGasInvoke),
            Jobs = section.GetSection(nameof(Jobs)).Get<CronJobSettings[]>() ?? Default.Jobs,
        };
    }
}

public class CronJobSettings
{
    public string Name { get; set; }
    public string Expression { get; set; }
    public bool RunOnce { get; set; }
    public CronJobContractSettings Contract { get; set; }
    public CronJobWalletSettings Wallet { get; set; }
}

public class CronJobContractSettings
{
    public string ScriptHash { get; set; }
    public string Method { get; set; }
    public CronJobContractParameterSettings[] Params { get; set; }
}

public class CronJobContractParameterSettings
{
    public string Type { get; set; }
    public string Value { get; set; }
}

public class CronJobWalletSettings
{
    public string Path { get; set; }
    public string Password { get; set; }
    public string Account { get; set; }
}
