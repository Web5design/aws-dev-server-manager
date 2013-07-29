AWS DEV Server Manager
======================

Simple utility for starting and stopping AWS servers 

### GUI

Shows a list of AWS servers, their running status and a button to Start or Stop them
![GUI screenshot](https://f.cloud.github.com/assets/227505/833497/156ade36-f2a3-11e2-90f4-8f7fba1e09e3.png)

### CLI

Enables you to start / stop AWS servers from the command line.

```
aws-dev-server-manager-cli --action=stop --instances="i-272586d,i-234586d,i-643586d,i-432586d"
```

Useful to run as a Scheduled build on your Build Server to shut down AWS dev instances at the end of the work day.

### Config

Add your `AWS_ACCESS_KEY`, `AWS_SECRET_KEY` and `AWS_REGION` to the `App.Config`
