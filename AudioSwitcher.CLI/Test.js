var devices = AudioSwitcher.getAudioDevices();

var defaultDevice;

for (var i = 0; i < devices.length; i++) {
    console.log(devices[i].id + " - " + devices[i].name + " - " + devices[i].flags);
    console.log("    Is Default: " + devices[i].isDefault);
    console.log("    Is Default Communications: " + devices[i].isDefaultComm);

    if (devices[i].isDefault && devices[i].flags == 1) {
        defaultDevice = devices[i];
    }
}

if (defaultDevice !== undefined) {

    var vol = defaultDevice.volume();

    //Sets the default device
    defaultDevice.setAsDefaultDevice();
    defaultDevice.setAsDefaultCommDevice();

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
}