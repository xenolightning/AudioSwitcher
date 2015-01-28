Audio Switcher CLI
=================

Blurb
------

A command executable exe, that processes AudioSwitcher scripting files. The scripting files are plain old Javascript files, but have support from the automatically available AudioSwitcher Scripting library.

The scripting library provides a limited set of functionality allowing Javascript to interact directly with any and all of the available Windows Audio Devices. This could be Speakers, Headphones, Microphones, Digital Outputs etc...

- - -

Usage
------

To run and execute a macro script use the following, where inputFile.js is the macro.

```
ascli.exe inputFile.js
```

To test/debug any scripts you are creating use the --debug switch. This will result in your Windows System Device being UNAFFECTED by the script. It runs the functions in a "sandbox"

```
ascli.exe --debug inputFile.js
```

- - -

Scripting Library
-------

By default when you execute a Javascript file using the Scripting Library it exposes _three_ libraries.

Libraries:
- AudioSwitcher
- Core
- console

- - -

### console ###

This exposes functionality similar to that seen in the Firebug Console. It allows the developer to debug their macro script by printing text to the output window.

#### Available Functions ####

    log
    debug
    info
    warn
    error
    assert
    group
    groupEnd
    time
    timeEnd
    
See the [Firebug Console API](https://getfirebug.com/wiki/index.php/Console_API) for more details.

- - -

### Core ###

Exposes a set of functions that may be useful in scripting. All function are contained within the "Core" namespace.

#### Available Functions ####

Member|Type|Arguments (type)|Returns|Description
:-----|:---|:---------------|:------|:--------
sleep|function|milliseconds (number)|void|Will force the Scripting engine to pause execution for _at least_ the number of milliseconds specified 

- - -

### AudioSwitcher ###

Exposes the low level functions for interacting with the Audio Switcher API, and therefore control Windows Sound Devices. All functions are contained within the "AudioSwitcher" namespace.

#### Available Functions ####

Member|Type|Arguments (type)|Returns|Description
:-----|:---|:---------------|:------|:--------
getAudioDevices|function|flags (flags)|AudioDevice[]|Returns all of the AudioDevices available, matching the input flags (0/1/2)
getAudioDevice|function|name (string)|AudioDevice|Returns the AudioDevice which has this name eg. "Speakers"
getPreferredDevices|function|flags (flags)|AudioDevice[]|Not Implemented
nextPreferredDevice|function||bool|Not Implemented
previousPreferredDevice|function||bool|Not Implemented
    
#### Types ####

##### Flags ######
Anywhere that the type "flags" is references it refers to the type of the device:
- Output - 1 - E.g "Speakers"
- Input - 2 - E.g "Microphone"
- All - 0 - There are certain situations where you can pass 0 to get ALL devices Input/Output


##### AudioDevice ######

Member|Type|Arguments (type)|Returns|Description
:-----|:---|:---------------|:------|:--------
id|field||string|The unique identifier for this device
name|field||string|The short name for this device eg. "Speakers"
flags|field||flags|The type of the device eg. 1 or 2
isDefault|field||bool|Whether this device is set as the Default Device
isDefaultComm|field||bool|Whether this device is set as the Default Communcations Device
volume|function|\[level\]\(number\)|number|Gets/Sets the volume level of this device. Optional parameter, if specfied - set volume
mute|function|mute(bool)|bool|Sets the muted state of this device. true to mute, false to unmute. Returns the resultant mute state
toggleMute|function||bool|Toggles the mute state of this device. Returns the resultant mute state.
setAsDefaultDevice|function||bool|Sets this device as the Default Device
setAsDefaultCommDevice|function||bool|Sets this device as the Default Communcations Device
    
- - -

Sample Macro Script
-------

Sample macro script that demonstrates the usage of some scripting functions.

```javascript
var devices = AudioSwitcher.getAudioDevices();
console.log("Number of devices: " + devices.length);
var defaultDevice;

for (var i = 0; i < devices.length; i++) {
    console.log(devices[i].id);
    console.log(devices[i].fullName);
    console.log("    Type: " + devices[i].type);
    console.log("    State: " + devices[i].state);
    console.log("    Is Playback (property): " + devices[i].isPlayback);
    console.log("    Is Playback (calculated): " + (devices[i].type == AudioSwitcher.DeviceType.PLAYBACK));
    console.log("    Is Default: " + devices[i].isDefault);
    console.log("    Is Default Communications: " + devices[i].isDefaultComm);
    console.log();

    if (devices[i].isDefault && devices[i].isPlayback) {
        defaultDevice = devices[i];
    }
}

var defDevById = AudioSwitcher.getAudioDevice(defaultDevice.id);
console.log("Id lookup successful: " + (defDevById !== undefined && defDevById.id === defaultDevice.id) + "\r\n");

console.log("Trying to get Speakers...");
var ad = AudioSwitcher.getAudioDevice("Speakers", AudioSwitcher.DeviceType.PLAYBACK);

if (ad !== null) {
    console.log(ad.fullName);
} else {
    console.log("Playback Speakers not found");
}
console.log();

if (defaultDevice !== undefined) {

    var vol = defaultDevice.volume();
    var isMuted = defaultDevice.isMuted;
    if (isMuted) {
        console.log("Device is muted");
    } else {
        console.log("Device is NOT muted");
    }

    //Sets the default device
    defaultDevice.setAsDefault();
    defaultDevice.setAsDefaultComm();

    console.log("Current Volume: " + vol);
    console.log("Set Volume To: " + defaultDevice.volume(10));
    Core.sleep(2000);
    console.log("Set Volume To: " + defaultDevice.volume(vol));
    Core.sleep(2000);
    console.log("Setting Mute to false");
    defaultDevice.mute(false);
    Core.sleep(2000);
    console.log("Toggling Mute");
    console.log("Is Muted: " + defaultDevice.toggleMute());
    Core.sleep(2000);
    console.log("Toggling Mute");
    console.log("Is Muted: " + defaultDevice.toggleMute());
    Core.sleep(2000);
    console.log("Setting Muted to [" + defaultDevice.mute(isMuted) + "]");
}
```
