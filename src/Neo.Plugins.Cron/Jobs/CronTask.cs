// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.Wallets;

namespace Neo.Plugins.Cron.Jobs;

internal class CronTask : ICronJob
{
    public string Name { get; private init; }
    public string Expression { get; private init; }
    public CronContract Contract { get; private init; }
    public Wallet Wallet { get; private init; }
    public UInt160 Sender { get; private init; }

    public static CronTask Create(CronJobSettings settings) =>
        new()
        {
            Name = settings.Name,
            Expression = settings.Expression,
            Contract = new(UInt160.Parse(settings.Contract.ScriptHash), settings.Contract.Method, settings.Contract.Params),
            Wallet = Wallet.Open(settings.Wallet.Path, settings.Wallet.Password, CronPlugin.NeoSystem.Settings),
            Sender = UInt160.Parse(settings.Wallet.Account),
        };

    public Task Run(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;
        WalletUtils.MakeAndSendTx(this);
        return Task.CompletedTask;
    }
}
