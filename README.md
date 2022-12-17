
# ViLA

[![.NET 6 CI build](https://github.com/charliefoxtwo/ViLA/actions/workflows/ci-build.yml/badge.svg?branch=develop)](https://github.com/charliefoxtwo/ViLA/actions/workflows/ci-build.yml)
[![GitHub](https://img.shields.io/github/license/charliefoxtwo/ViLA?style=flat-square)](LICENSE)
[![Discord](https://img.shields.io/discord/840762843917582347?style=flat-square)](https://discord.gg/rWAF3AdsKT)


ViLA (**Vi**rpil **L**ED **A**utomator) is an extensible tool for configuring your Virpil usb device's LEDs to react to certain events. It can be extended via plugins written by anybody, which can do things like tail log files, pair with DCS-BIOS, and more. These plugins send messages to ViLA, which are parsed according to its configuration file.

<img src="https://thumbs.gfycat.com/ResponsibleFearlessIceblueredtopzebra-size_restricted.gif" height="400" /> <img src="https://thumbs.gfycat.com/WateryUnevenBarasingha-size_restricted.gif" height="400" /> <img src="https://thumbs.gfycat.com/ColossalEmbellishedFattaileddunnart-size_restricted.gif" height="400" />


## Getting started

Check out the [wiki](https://github.com/charliefoxtwo/ViLA/wiki) for a getting started guide!


## Recommended plugins

- [DCS Bios Reader](https://github.com/charliefoxtwo/ViLA-DCS-BIOS-Reader)
- [SimConnect Reader](https://github.com/pieterwasalreadytaken/ViLA-SimConnect-Reader)


## Running

Just double-click ViLA.exe to start ViLA. It's important to note that ViLA isn't very useful without plugins, so find some good ones!


## What's next?

ViLA has a long way to go. 

 - If you think you've found a bug, open an issue in the [Issues](https://github.com/charliefoxtwo/ViLA/issues) section
 - If you have a question or need support, try the [Discussions Q&A](https://github.com/charliefoxtwo/ViLA/discussions/categories/q-a) or our [Discord](https://discord.gg/rWAF3AdsKT)
 - If you have an idea for a new feature for ViLA, please share it in the [Ideas](https://github.com/charliefoxtwo/ViLA/discussions/categories/ideas) Discussion section!
 - If you've done something cool with ViLA, please show us in the [Show and Tell](https://github.com/charliefoxtwo/ViLA/discussions/categories/show-and-tell) Discussion section!
 - If you're working on a plugin for ViLA, please get in touch either in the [Discussions](https://github.com/charliefoxtwo/ViLA/discussions) tab or on [Discord](https://discord.gg/rWAF3AdsKT)! You're free to work on whatever you want, I'd just love to see what ideas everybody comes up with!


## FAQ

#### Does ViLA offer any protections to make sure extensions don't do bad things to my computer?

**No**. It's very important to understand this. **You and you alone are responsible for whatever happens to your computer as a result of extensions you install.** Any code at all can be written in an extension. The developers of ViLA will not be held responsible for malicious plugins. When in doubt, **DO NOT INSTALL** a plugin.

#### Does ViLA support additional conditional logic like AND, OR, etc?

Yes! You can nest logical operators as deep as you like.

#### Does ViLA support more complex actions, like flashing a light in response to an action?

Not directly, no. You can get creative with the plugin and the Ids in your config.json file, but generally speaking this is not officially supported at this time. Maybe at some point in the future!


## Local Development

If making package changes (highly discouraged) it is important to regenerate the plugin_manifest.xml file. There used to be a tool to do this, but development of it stopped with .NET 5 and so the file must now be maintained by hand. Hooray!

## Acknowledgements

 - [readme tools](https://readme.so)
 - [badges](https://shields.io)
