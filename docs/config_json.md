# Config File
Is placed in the same directory as `Crontab.dll`. Located in `Plugins\Crontab`
directory.

## Root Section
| Property | Type | Description |
| ---: | :---: | :--- |
|PluginConfiguration|object|note: Standard for `NEO` plugins|

## PluginConfiguration Section
| Property | Type | Description |
| ---: | :---: | :--- |
|Network|uint32|Network you want to execute tasks on.|
|MaxGasInvoke|int64|Max gas allow on the `NEO` `VM`.|
|Job|object|see [job](#job-section) section for more details.|

## Job Section
| Property | Type | Description |
| ---: | :---: | :--- |
|Path|string|Where your `*.job` files are. Defaults to `jobs` folder in the `neo-cli` root directory. _note: You must create this folder_.|
|Timeout|uint32|Max time in seconds that a job can run for. It's recommended that it should be no more than 30 seconds.
