// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.Ledger;
using Neo.Plugins.Cron.Jobs;

namespace Neo.Plugins.Cron;

public partial class CronPlugin : Plugin
{
    public override string Name => "Crontab";
    public override string Description => "Crontab task scheduler for executing blockchain tasks.";

    internal static NeoSystem NeoSystem { get; private set; }

    private readonly CronScheduler _scheduler;

    public CronPlugin()
    {
        _scheduler = new();
    }

    public override void Dispose()
    {
        _scheduler.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void Configure() =>
        CronPluginSettings.Load(GetConfiguration());

    protected override void OnSystemLoaded(NeoSystem system)
    {
        if (system.Settings.Network != CronPluginSettings.Current.Network)
            return;
        NeoSystem = system;
        CronPluginSettings.Current.Jobs.ToList().ForEach(CreateJob);
    }
}
