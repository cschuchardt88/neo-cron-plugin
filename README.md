# neo-cron-plugin
Crontab task scheduler for executing blockchain tasks.

## Features
- Crontab Scheduler
- Invoke Contracts
- Invoke Transfers (_Nep17_)

# Install
This plugin requires at least `neo-cli` version
[3.6.0](https://github.com/neo-project/neo-node/releases). After you
download and extract the `.zip` file.

**Next Steps**
1. _Create a folder in the `Plugins` directory called `Crontab`._
1. _Copy & Paste `Crontab.dll`, `config.json` and `NCrontab.dll` into `Plugins\Crontab` directory._
1. _Edit `config.json` with your configuration. [More datails](/docs)_

# Example Tasks
You can find more datails on how to configure [jobs here](/docs/jobs.md).

- Invoke a [contract](/examples/HelloInvokeMethod.job)
- Invoke a [transfer](/examples/HelloTransfer.job)
