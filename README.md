Audio Switcher
=============

.NET Library which facilitates interacting with Audio Devices on Windows


Components
======


### [Audio Api](AudioSwitcher.AudioApi)

The "heavy lifting", this is the library that actually makes the system calls, and can be used in any .NET 4 application and above.


### [Scripting](AudioSwitcher.Scripting)

Uses the Audio Api, and Jurassic to create a Javascript interpreter/engine. Enables the ability to query and alter audio devices using Javascript.


### [Command Line Interface](AudioSwitcher.CLI)

A simple CLI over the Scripting Library, which processes Javascript files, and runs them against the current system. Full documentation is in the [README](AudioSwitcher.CLI/README.md).
