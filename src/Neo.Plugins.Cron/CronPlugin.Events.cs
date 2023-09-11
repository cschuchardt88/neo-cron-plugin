// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;

namespace Neo.Plugins.Cron;

public partial class CronPlugin
{
    private void OnBlockchainCommitted(NeoSystem system, Block block)
    {
        if (NeoSystem.Settings.Network != CronPluginSettings.Current.Network)
            return;
    }

    private void OnBlockchainCommitting(NeoSystem system, Block block, DataCache snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
    {
        if (NeoSystem.Settings.Network != CronPluginSettings.Current.Network)
            return;
    }
}
