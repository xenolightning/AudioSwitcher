Audio Switcher
=============

.NET Library which facilitates interacting with Audio Devices on Windows


Components
======


### [Audio Api](AudioSwitcher.AudioApi)

An abstracted set of classes that can be extended to interact with any audio system. Is intended to be used across Windows/Windows RT/Windows Phone

### [Core Audio Api](AudioSwitcher.AudioApi.CoreAudio)

The MMDeviceAPI integration layer. This is the low level COM library that it used to interact with Audio Devices on a PC running Windows Vista or later.


### [PowerShell Module](https://github.com/xenolightning/AudioSwitcher.PowerShell)

A powershell wrapper module over CoreAudioApi.
Full documentation is in the [README](AudioSwitcher.PowerShell.CoreAudio/README.md).


### [Scripting](AudioSwitcher.Scripting)

Uses the Audio Api, and Jurassic to create a Javascript interpreter/engine. Enables the ability to query and alter audio devices using Javascript.


### [Command Line Interface](Samples/AudioSwitcher.CLI)

A simple CLI over the Scripting Library, which processes Javascript files, and runs them against the current system. Full documentation is in the [README](Samples/AudioSwitcher.CLI/README.md).
