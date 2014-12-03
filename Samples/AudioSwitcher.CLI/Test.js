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

if (ad !== undefined) {
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
    Core.sleep(2000);
}