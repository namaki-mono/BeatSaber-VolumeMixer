# Volume Mixer
A Beat Saber mod that allows the user to change the volume of applications in-game from the main menu

![image](https://github.com/namaki-mono/BeatSaber-VolumeMixer/assets/81204441/b4eb2eb9-f0c0-4907-998c-c5007c1ab3bf)
![image](https://github.com/namaki-mono/BeatSaber-VolumeMixer/assets/81204441/1f583a42-75fb-49b3-a377-db10f78e6ac1)


The first 4 characters identify the device, and the characters following the slash indicate which source's volume will be modified

## Installation
This mod depends on BSIPA 4.2.0 or above, which can be installed from [ModAssistant](https://github.com/Assistant/ModAssistant). It is likely already installed

Download the latest version of the mod [here](https://github.com/namaki-mono/BeatSaber-VolumeMixer/releases), and extract the .zip file to the `Beat Saber` folder. `svcl.exe` should be in the top-level folder, and `VolumeMixer.dll` should be in the `Beat Saber/Plugins` folder

## Configuration
Edit `Beat Saber\UserData\VolumeMixer.json` (this file will be created automatically when the game is launched with this mod installed)

There is no in-game settings page at the moment. This may be added in the future

### Configuration Options
- `Enabled` - Toggles the mod on or off
- `ShowHandle` - Allows the user to move the volume mixer UI
- `UIPosition` - The position of the UI in the main menu
- `UIRotation` - The rotation of the UI in the main menu

## FAQ
- Q: My sound source is not appearing in the list, how do I make it appear?
- A: Try selecting a different source, then checking the list again. The list refreshes after each sound source change

## Credits
- [SoundVolumeView](https://www.nirsoft.net/utils/sound_volume_view.html) - System-level volume control
