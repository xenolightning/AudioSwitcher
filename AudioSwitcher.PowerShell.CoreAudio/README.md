Core Audio Powershell Module
----

Installation
------

Using PsGet enter the following command. 

```
Install-Module -ModuleUrl https://github.com/xenolightning/AudioSwitcher/releases/download/1.0.0.0/AudioSwitcher.zip
```

Alternatively download the latest zip package and install it manually



Usage
------

Currently two commands are available: 

Returns a list of devices in the system
```
Get-AudioDevices [-Type All|Playback|Capture]
```

Returns a specific device in the system
```
Get-AudioDevice [-Id (Guid)] [-Name (string - support wildcards)]
```
