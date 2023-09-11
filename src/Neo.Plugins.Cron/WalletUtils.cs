// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Akka.Actor;
using Neo.ConsoleService;
using Neo.Network.P2P.Payloads;
using Neo.Plugins.Cron.Jobs;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;

namespace Neo.Plugins.Cron;

internal static class WalletUtils
{
    public static void MakeAndSendTx(CronTask cronTask)
    {
        if (cronTask != null || (cronTask.Wallet != null && cronTask.Sender != null))
        {
            try
            {
                var tx = new Transaction()
                {
                    Signers = new[] { new Signer() { Account = cronTask.Sender, Scopes = WitnessScope.CalledByEntry } },
                    Attributes = Array.Empty<TransactionAttribute>(),
                    Witnesses = Array.Empty<Witness>(),
                };
                if (OnInvoke(cronTask.Contract, tx) == false)
                    ConsoleHelper.Error($"Cron:Job[\"{cronTask.Name}\"]::\"Virtual machine invoke failed.\"");
                else
                {
                    tx = cronTask.Wallet.MakeTransaction(CronPlugin.NeoSystem.StoreView, tx.Script, cronTask.Sender, tx.Signers, maxGas: CronPluginSettings.Current.MaxGasInvoke);
                    SignAndSendTx(cronTask.Wallet, tx);
                }
            }
            catch (InvalidOperationException)
            {
                ConsoleHelper.Error($"Cron:Job[\"{cronTask.Name}\"]::\"Transaction failed.\"");
            }
        }
    }

    public static bool OnInvoke(CronContract cronContract, Transaction tx)
    {
        var contract = NativeContract.ContractManagement.GetContract(CronPlugin.NeoSystem.StoreView, cronContract.ScriptHash);
        if (contract == null)
            return false;
        else
        {
            if (contract.Manifest.Abi.GetMethod(cronContract.Method, 0) == null)
                return false;
            else
            {
                using var sb = new ScriptBuilder()
                    .EmitDynamicCall(cronContract.ScriptHash, cronContract.Method);

                tx.Script = sb.ToArray();

                using var engine = ApplicationEngine.Run(
                    tx.Script, CronPlugin.NeoSystem.StoreView,
                    tx, settings: CronPlugin.NeoSystem.Settings,
                    gas: CronPluginSettings.Current.MaxGasInvoke);

                return engine.State != VMState.FAULT;
            }
        }
    }

    public static void SignAndSendTx(Wallet wallet, Transaction tx)
    {
        var context = new ContractParametersContext(CronPlugin.NeoSystem.StoreView, tx, CronPlugin.NeoSystem.Settings.Network);
        if (wallet.Sign(context) && context.Completed)
        {
            tx.Witnesses = context.GetWitnesses();
            CronPlugin.NeoSystem.Blockchain.Tell(tx);
        }
    }
}
