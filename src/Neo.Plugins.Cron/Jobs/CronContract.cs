// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

namespace Neo.Plugins.Cron.Jobs;

internal class CronContract
{
    public UInt160 ScriptHash { get; private set; }
    public string Method { get; private set; }

    public CronContract(
        UInt160 scriptHash,
        string method)
    {
        ScriptHash = scriptHash;
        Method = method;
    }
}