# Jobs
In this section you will learn how to create and configure job(s).

## Create a Job
Add `*.job` file to the path of `JobsPath` in the _config.json_. In this
location the plugin will pull all files; including ones from other
directories.


## Template (_*.job_) File
```json
{
  "Type": "",
  "Name": "",
  "Expression": "",
  "RunOnce": false,
  "Wallet": {
    "Path": "",
    "Password": "",
    "Account": ""
  },
  "Contract": {
    "ScriptHash": "",
    "Method": "",
    "Params": []
  }
}
```

## Job Config
| Property | Type | Description |
| ---: | :---: | :--- |
|Type|enum|`Basic` or `Transfer`|
|Name|string|is the name of the job.|
|Expression|string|is the crontab expression.|
|RunOnce|boolean|Only execute the job once.|
|Contract|object|See [details](#contract-section).|
|Wallet|object|See [details](#wallet-section).|

## Contract Section
| Property | Type | Description |
| ---: | :---: | :--- |
|ScriptHash|string|Smart contract `hash160`.|
|Method|string|Smart contract method/function that gets invoked at the given timeframe.|
|Params|array|is an array of parameter objects. see [details](#contract-parameters).|

## Wallet Section
| Property | Type | Description |
| ---: | :---: | :--- |
|Path|string|file path plus filename with extension. _note: path is relative to where file neo-cli.exe_|
|Password|string|Password to open wallet file.|
|Account|string| `hash160` of the account in the wallet. _note: Transactions will preformed with this account._|

## Contract Parameters
| Property | Type | Description |
| ---: | :---: | :--- |
|Type|enum| see [details](#contract-parameter-enum).|
|Value|string| string format of the value type.|

## Contract Parameter Enum
Arrays and Maps are not currently supported. That kind of data
should be processed in the smart contract. You want to keep cost
per `byte` down for transactions.

| Type | Format | Comment |
| ---: | :---: | :--- |
|ByteString|`Base64`||
|Signature|`Base64`||
|Boolean|`Bool`|`True` or `False`|
|Integer|`BigInt`||
|String|`utf-8`||
|Hash160|`string`|_note: valid? depends if contract checks for validation_.|
|Hash256|`string`||
|PublicKey|`hexstring`||
