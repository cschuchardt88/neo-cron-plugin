# Jobs
In this section you will learn how to create and configure job(s).

## Create a Job
Create a create file with any name with `.job` extension. Place file in
`JobPath` location directory from [_config.json_](/docs/config_json.md) file.

## Basic Template (_*.job_) File
```json
{
  "Type": "",
  "Name": "",
  "Expression": "",
  "RunOnce": false,
  "Wallet": {
    "Path": "",
    "Password": "",
    "Account": "",
    "Signers": []
  },
  "Contract": {
    "ScriptHash": "",
    "Method": "",
    "Params": []
  }
}
```

## Transfer Template (_*.job_) File
```json
{
  "Type": "Transfer",
  "Name": "",
  "Expression": "",
  "RunOnce": false,
  "Wallet": {
    "Path": "",
    "Password": "",
    "Account": "",
    "Signers": []
  },
  "Transfer": {
    "AssetId": "",
    "SendTo": "",
    "SendAmount": 0.00,
    "Comment": null
  }
}
```

## Root Section
| Property | Type | Description |
| ---: | :---: | :--- |
|Type|enum|`Basic` or `Transfer`|
|Name|string|Name of the job.|
|Expression|string|Crontab schedule expression. In `5` or `6` part format.|
|RunOnce|boolean|**Only** execute the job once.|
|Contract|object|See [details](#contract-section).|
|Wallet|object|See [details](#wallet-section).|

## Transfer Section
| Property | Type | Description |
| ---: | :---: | :--- |
|AssetId|string|Smart contract hash. As `ScrtipHash` format.|
|SendTo|string|**NEO** Wallet address. As `ScriptHash` format|
|SendAmount|decimal|Non-negative number.|
|Comment|string|`data` field in a transfer method. Unsure? Leave blank.|

## Contract Section
| Property | Type | Description |
| ---: | :---: | :--- |
|ScriptHash|string|Smart contract `hash160`.|
|Method|string|Smart contract method/function that gets invoked at the given timeframe.|
|Params|array|is an array of parameter objects. see [details](#contract-parameters).|

## Wallet Section
| Property | Type | Description |
| ---: | :---: | :--- |
|Path|string|Full file name path. Path is relative to where `neo-cli.exe` is.|
|Password|string|Password to open wallet file.|
|Account|string|`hash160` of the account in the wallet. Transactions will be preformed with this account as sender and witness.|
|Signers|Array|An Array of strings as ScriptHash `hash160`. Defaults to wallet account.|

## Contract Parameters
| Property | Type | Description |
| ---: | :---: | :--- |
|Type|enum|see [details](#contract-parameter-enum).|
|Value|string|string format of the value type.|

## Contract Parameter Enum
Arrays and Maps are not currently supported. That kind of data
should be processed in the smart contract. You want to keep cost
per `byte` down for transactions.

| Type | Format | Comment |
| ---: | :---: | :--- |
|ByteString|`Base64`||
|Signature|`Base64`||
|Boolean|`Boolean`|Value must be `True` or `False` in string form.|
|Integer|`BigInt`|**Only** `numbers` in string form up to 32 characters long.|
|String|`utf-8`|Make sure you use `utf-8` text encoding.|
|Hash160|`hexstring`|`ScriptHash` format with prefix `0x`|
|Hash256|`hexstring`| `SHA-256` hash starting with prefix `0x`.|
|PublicKey|`hexstring`|`ECPoint` `Secp256r1` public key in hex format with no prefix.|
