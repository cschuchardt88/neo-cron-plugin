<p align="center" width="100%">
    <img src="https://github.com/cschuchardt88/neo-cron-plugin/blob/master/imgs/logo-transparent.png" alt="Crontab-logo" />
</p>

```bash
*    *    *    *    *       Crontab Expression Chart
│    │    │    │    │
│    │    │    │    │
│    │    │    │    |_________ Day of Week (0 – 6) (0 is Sunday)
│    │    │    |____________ Month (1 – 12), * means every month
│    │    |______________ Day of Month (1 – 31), * means every day
│    |________________ Hour (0 – 23), * means every hour
|___________________ Minute (0 – 59), * means every minute
```

<p align="center" width="100%">
    <a href="https://github.com/cschuchardt88/neo-cron-plugin/blob/master/LICENSE">
        <img src="https://img.shields.io/badge/license-MIT-green" alt="license-MIT" />
    </a>
    <a href="https://github.com/cschuchardt88/neo-cron-plugin/tags">
        <img src="https://img.shields.io/github/v/tag/cschuchardt88/neo-cron-plugin" alt="neo-cron-plugin-tags" />
    </a>
    <a href="https://github.com/cschuchardt88/neo-cron-plugin/releases">
        <img src="https://img.shields.io/github/downloads/cschuchardt88/neo-cron-plugin/total" alt="neo-cron-plugin-releases-downloads" />
    </a>
</p>

# neo-cron-plugin
Task scheduler for sending transactions to the blockchain. Just as the
name implies `Crontab` does just that! Schedule jobs to invoke contracts
or transfer funds at certain times of the day, month, year, hour and
minute.

## Features
- Task Scheduler
- Manage jobs in `cli` console.
- Send transaction types.
  - Invoke Contract Methods
  - Send Nep-17 Transfers

## Upcoming Features
- Send `VM` scripts in transactions.
- Detailed error reporting.
- enable/disable jobs in their config file.

Have a feature you want to recommend for this project. Just create an
[issue](https://github.com/cschuchardt88/neo-cron-plugin/issues). 

# Install
This plugin requires at least `neo-cli` version
[3.6.0](https://github.com/neo-project/neo-node/releases). After you
download and extract the `.zip` file.

**Next Steps**
1. _Open `neo-cli` directory._
1. _Create a folder in the `Plugins` directory called `Crontab`._
1. _Copy & Paste `Crontab.dll`, `config.json` and `NCrontab.dll` into `Plugins\Crontab` directory._
1. _Edit `config.json` with your configuration. [More details](/docs/CONFIG_JSON.md)_

# Example Tasks
You can find more details on how to create and configure jobs [here](/docs/JOBS.md).

**Schedule Job Examples**
- [contract](/examples/HelloInvokeMethod.job)
- [transfer](/examples/HelloTransfer.job)
