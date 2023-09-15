// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Neo.Network.P2P.Payloads;
using Neo.Wallets;

namespace Neo.Plugins.Crontab.Jobs;

internal class CronTransferJob : ICronJob
{
    public CronJobType Type => CronJobType.Transfer;
    public string Name { get; init; }
    public string Expression { get; init; }
    public UInt160 TokenHash { get; init; }
    public UInt160 SendTo { get; init; }
    public decimal SendAmount { get; init; }
    public Signer[] Signers { get; init; }
    public string Comment { get; init; }
    public Wallet Wallet { get; init; }
    public UInt160 Sender { get; init; }

    public static CronTransferJob Create(CronJobTransferSettings settings) =>
        new()
        {
            Name = settings.Name,
            Expression = settings.Expression,
            TokenHash = UInt160.Parse(settings.Transfer.AssetId),
            SendTo = UInt160.Parse(settings.Transfer.SendTo),
            SendAmount = decimal.Parse(settings.Transfer.SendAmount),
            Sender = UInt160.Parse(settings.Wallet.Account),
            Signers = settings.Transfer.Signers == null || settings.Transfer.Signers.Length == 0 ?
                new[] { new Signer() { Account = UInt160.Parse(settings.Wallet.Account), Scopes = WitnessScope.CalledByEntry } } :
                settings.Transfer.Signers.Select(s => new Signer() { Account = UInt160.Parse(s), Scopes = WitnessScope.CalledByEntry }).ToArray(),
            Comment = settings.Transfer.Comment,
            Wallet = Wallet.Open(settings.Wallet.Path, settings.Wallet.Password, CronPlugin.NeoSystem.Settings),
        };

    public void Run()
    {
        WalletUtils.MakeTransferAndSendTx(this);
    }
}
