Core Audio Powershell Module
----

Installation
------

Using PsGet enter the following command. 

```
Install-Module -ModuleUrl https://github.com/xenolightning/AudioSwitcher/releases/download/1.0.0.0/AudioSwitcher.zip
```

Alternatively:

1. Download the latest zip package
2. Extract to %User%\Documents\WindowsPowershell\Modules\AudioSwitcher

3. Run `Install-Module AudioSwitcher`
```
\---Modules
    +---AudioSwitcher
    |       AudioSwitcher.AudioApi.CoreAudio.dll
    |       AudioSwitcher.AudioApi.dll
    |       AudioSwitcher.PowerShell.CoreAudio.dll
    |       AudioSwitcher.psd1
```


Usage
------

Currently three commands are available: 


Returns a reference to an audiocontroller object. This audio controller can do everything!
```
Get-AudioController
```

Returns a list of devices in the system
```
Get-AudioDevices [-Type All|Playback|Capture]
```

Returns a specific device in the system
```
Get-AudioDevice [-Id (Guid)] [-Name (string - support wildcards)]
```
