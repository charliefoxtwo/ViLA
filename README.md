
# ViLA

[![.NET 5 CI build](https://github.com/charliefoxtwo/ViLA/actions/workflows/ci-build.yml/badge.svg?branch=develop)](https://github.com/charliefoxtwo/ViLA/actions/workflows/ci-build.yml)
![GitHub](https://img.shields.io/github/license/charliefoxtwo/ViLA?style=flat-square)
![Discord](https://img.shields.io/discord/840762843917582347?style=flat-square)


ViLA (**Vi**rpil **L**ED **A**utomator) is an extensible tool for configuring your Virpil usb device's LEDs to react to certain events. It can be extended via plugins written by anybody, which can do things like tail log files, pair with DCS-BIOS, and more. These plugins send messages to ViLA, which are parsed according to its configuration file.

<img src="https://thumbs.gfycat.com/ResponsibleFearlessIceblueredtopzebra-size_restricted.gif" height="400" />
<img src="https://thumbs.gfycat.com/WateryUnevenBarasingha-size_restricted.gif" height="400" />
<img src="https://thumbs.gfycat.com/ColossalEmbellishedFattaileddunnart-size_restricted.gif" height="400" />

## Installation

Download the latest version from the Releases page, and place it in its own folder wherever you want to keep it. You'll need two things:

1. [`config.json`](#configjson)
1. `Plugins/` folder (where your plugins will go)


## Installing Plugins

To install a plugin, just drop it in its own folder within the `Plugins/` folder. Your file tree should look something like this:

```
ViLA/
|-- Plugins/
|   |-- Plugin A/
|   |   `-- <dll files>
|   `-- Plugin B/
|       `-- <dll files>
|-- config.json
`-- ViLA.exe
```
Once there, it should be automatically loaded on the next run of ViLA.

Some plugins include:

- [DCS Bios Reader](https://github.com/charliefoxtwo/ViLA-DCS-BIOS-Reader)

## config.json

The config file tells the program what color to set what led for what action. The config files use jsonschema, so it's recommended to edit them with a text editor that can provide type hints for json files with a defined schema (e.g. VS Code).

It's structured like so:
```json5
{
    // schema at the top
    "$schema": "https://raw.githubusercontent.com/charliefoxtwo/ViLA/develop/ViLA/Configuration/Schema/ActionConfiguration.json.schema",
    // devices, identified by hex code PID 
    "devices": {
        "825B": [
            // array of actions for the device
            // ...
        ]
    }
}
```

The following is an example of a device action:
```json5
{
    // color to set the LED to if the trigger succeeds
    "color": "ff8000",
    "trigger": {
        // id to test trigger
        "biosCode": "MASTER_CAUTION_LT",
        // value to compare sent value to
        "value": 1,
        // how to compare the sent value to this value (<sent> <comparator> <value>)
        "comparator": "EqualTo"
    },
    // the target led to change, if successful
    "target": {
        // led number
        "ledNumber": 1,
        // location of led
        "boardType": "OnBoard"
    }
}
```


## Running

Just double-click ViLA.exe to start ViLA. It's important to note that ViLA isn't very useful without plugins, so find some good ones!


## FAQ

#### Does ViLA offer any protections to make sure extensions don't do bad things to my computer?

**No**. It's very important to understand this. **You and you alone are responsible for whatever happens to your computer as a result of extensions you install.** Any code at all can be written in an extension. The developers of ViLA will not be held responsible for malicious plugins. When in doubt, **DO NOT INSTALL** a plugin.

#### Does ViLA support additional conditional logic like AND, OR, etc?

Not at this time. Maybe at some point in the future though!

#### Does ViLA support more complex actions, like flashing a light in response to an action?

Not directly, no. You can get creative with the plugin and the Ids in your config.json file, but generally speaking this is not officially supported at this time. Again, maybe at some point in the future!


## Developing plugins

> If you have any experience developing .NET applications with plugins, please reach out to me! I'd love to find a way to make the plugin development process smoother, if possible.

It is recommended (but not required) to not package any dlls from the following packages in your plugin. Including them may cause dependency resolution issues. Since the project has copies of these dlls too, there's no need to include them anyway.
 - Virpil.Communicator (you shouldn't be using this anyway)
 - Virpil.Communicator.PluginBase
 - McMaster.NETCore.Plugins (you shouldn't be using this anyway... unless your extension has extensions? o.0)
 - Microsoft.Extensions.Logging.Abstractions
 - Microsoft.Extensions.Logging.Console (you shouldn't be using this anyway)
 - Newtonsoft.json
 - hidlibrary (you probably shouldn't be using this anyway)
 - Microsoft.DotNet.PlatformAbstractions
 - Microsoft.Extensions.Configuration
 - Microsoft.Extensions.Configuration.Abstractions
 - Microsoft.Extensions.Configuration.Binder
 - Microsoft.Extensions.DependencyInjection
 - Microsoft.Extensions.DependencyInjection.Abstractions
 - Microsoft.Extensions.DependencyModel
 - Microsoft.Extensions.Logging
 - Microsoft.Extensions.Logging.Configuration
 - Microsoft.Extensions.Options
 - Microsoft.Extensions.Primitives
 - System.Text.Json

The easiest way to do this is to [specify a manifest file](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) when running `dotnet publish`. You can use [plugin_manifest.xml](plugin_manifest.xml) in the root of this repository for exactly this purpose.


## Local Development

If making package changes (highly discouraged) it is important to regenerate the plugin_manifest.xml file. This can be done with the following command: `dotnet store -m ViLA.csproj --runtime win-x64 -f net5.0`

This command will output `%userprofile%/.dotnet/store/x64/net5.0/artifacts.xml`, among other things. Make sure to replace the current plugin_manifest.xml file with this one.

## Acknowledgements

 - [readme tools](https://readme.so)
 - [badges](https://shields.io)