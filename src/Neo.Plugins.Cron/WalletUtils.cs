// Copyright (C) 2023 Christopher R Schuchardt
//
// The neo-cron-plugin is free software distributed under the
// MIT software license, see the accompanying file LICENSE in
// the main directory of the project for more details.

using Akka.Actor;
using Neo.ConsoleService;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.Plugins.Cron.Jobs;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System.Numerics;

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
                if (OnInvokeMethod(cronTask.Contract, tx) == false)
                    ConsoleHelper.Error($"Cron:Job[\"{cronTask.Name}\"]::\"Virtual machine invoke method failed.\"");
                else
                {
                    tx = cronTask.Wallet.MakeTransaction(CronPlugin.NeoSystem.StoreView, tx.Script, cronTask.Sender, tx.Signers, maxGas: CronPluginSettings.Current.MaxGasInvoke);
                    SignAndSendTx(cronTask.Wallet, tx);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Cron:Job[\"{cronTask.Name}\"]::\"{ex.Message}\"");
            }
        }
    }

    public static bool OnInvokeMethod(CronContract cronContract, Transaction tx)
    {
        var contract = NativeContract.ContractManagement.GetContract(CronPlugin.NeoSystem.StoreView, cronContract.ScriptHash);
        if (contract == null)
            return false;
        else
        {
            if (contract.Manifest.Abi.GetMethod(cronContract.Method, cronContract.Params.Length) == null)
                return false;
            else
            {
                var args = cronContract.Params.Select(ConvertToContractParameter).ToArray();
                using var sb = new ScriptBuilder();
                if (args.Length > 0)
                    sb.EmitDynamicCall(cronContract.ScriptHash, cronContract.Method, args);
                else
                    sb.EmitDynamicCall(cronContract.ScriptHash, cronContract.Method);

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

    public static ContractParameter ConvertToContractParameter(CronJobContractParameterSettings parameterSettings)
    {
        return parameterSettings.Type.ToLowerInvariant() switch
        {
            "bytearray" => new()
            {
                Type = ContractParameterType.ByteArray,
                Value = Convert.FromBase64String(parameterSettings.Value),
            },
            "signature" => new()
            {
                Type = ContractParameterType.Signature,
                Value = Convert.FromBase64String(parameterSettings.Value),
            },
            "boolean" => new()
            {
                Type = ContractParameterType.Boolean,
                Value = bool.Parse(parameterSettings.Value),
            },
            "integer" => new()
            {
                Type = ContractParameterType.Integer,
                Value = BigInteger.Parse(parameterSettings.Value),
            },
            "string" => new()
            {
                Type = ContractParameterType.String,
                Value = parameterSettings.Value,
            },
            "hash160" => new()
            {
                Type = ContractParameterType.Hash160,
                Value = UInt160.Parse(parameterSettings.Value),
            },
            "hash256" => new()
            {
                Type = ContractParameterType.Hash256,
                Value = UInt256.Parse(parameterSettings.Value),
            },
            "publickey" => new()
            {
                Type = ContractParameterType.PublicKey,
                Value = ECPoint.Parse(parameterSettings.Value, ECCurve.Secp256r1),
            },
            _ => throw new NotSupportedException($"{parameterSettings.Type} is not supported.")
        };
    }
}
