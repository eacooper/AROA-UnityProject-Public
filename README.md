# AROA-Visual-Cues
Visual cues experimentation for Augmented Reality Obstacle Avoidance

This project establishes a visual cue system for the HoloLens 2, as used in the paper "Using augmented reality to cue obstacles for people with low vision." It uses Unity v2019.4.40f1 and the Windows Mixed Reality XR plug-in.

# Functionality

## Core Functionality

The core functions of the project are:
1. **Course Alignment** - Align with a predetermined [physical obstacle course layout](https://imgur.com/a/NXOB8BM) using [QR codes](https://docs.google.com/document/d/1AcF6-MDEuMgz8fVGd6OxW-DgXVH-ZRz1i57YcW5o8Vc/edit?usp=sharing). (QR codes created [here](https://www.qr-code-generator.com/).) This course is set up over a 1.8m x 15m portion of hallway and can take one of 8 layouts.
2. **Cue Display** - Display visual cues in one of three modes: World-Locked (aka Collocated), Heads-Up (aka HUD), or Combined (both other cue types at once).
3. **Performance Logging** - Log the participant's position and rotation over time as they walk the obstacle course.

Note that the obstacles used are: 
- Low (a 2” x 2” dark gray bar of foam spanning the hallway that participants stepped over)
- High (a ~2” wide white streamer hung horizontally at eye level that participants ducked under)
- Wide (a 2m tall x 1.5m wide folding room divider that participants walked around)

The QR code printout should be placed to the left of the start of the course and positions fine-tuned using voice commands or keyboard shortcuts.

## Secondary Functionality

The following secondary functions support the core functions.

1. Calibration Mode - displays all four sides of the heads-up/HUD cues and removes distance limitations on world-locked/collocated cues. Helpful for ensuring mode and obstacle placements are correct.
2. HUD Calibration - adjust the Heads-Up/HUD cues left or right and condense them for users with vision loss in one eye.
3. Debug Text - display a floating window with debugging information to facilitate setup.
4. Voice Commands - control the experience using voice commands or keyboard input from a bluetooth keyboard.


# Key Unity Elements and Associated Scripts
These can be found in the latest scene (currently Visual Cues v7b_Hallway).

* **Mixed Reality Toolkit** - the core component enabling HoloLens functionality. Read more at [Microsoft's GitHub](https://github.com/microsoft/MixedRealityToolkit-Unity). Note that speech commands and their associated keyboard shortcuts are entered here under Input > Speech > Speech Commands.

* **Main Camera** - The camera represents the HoloLens. Note that the Clipping Plane and Culling Mask fields are adjusted to affect world-locked cue visibility.

   * HUD Manager - This holds the **HUD_Manager** script, which controls the heads-up cues.
   
      * Text To Speech Object - This holds the **Text To Speech** script and an audio source, used to announce updates to user.
   
      * HUD Frame - This holds the HUD Cue North/East/South/West objects, which are what enable or disable to show heads-up cues.
      
      * Debug Canvas - This shows the debug text while enabled.
      
* Obstacle Manager - This holds three key scripts: **Obstacle Manager**, **Experiment Logger**, and **Speech Input Handler**. Note that any voice commands/keyboard shortcuts used in the MRTK must be matched to commands in Speech Input Handler. 

* QR Codes Manager - This holds two of the necessary scripts for QR code usage: **QR Codes Manager** and **QR Codes_AROA**.

* Collocated Cues - This holds the set of six potential obstacles, as well as a calibration obstacle used to help ensure placement is correct. 


* Other Objects - These are objects representing the floor and Calibration QR Code to help place obstacles. They are kept disabled during use.

# Building and Deploying to HoloLens

The process of building the unity project and deploying to HoloLens can be very delicate and sensitive to error. Here is what worked for us.

## Building

1. In Unity, go to File > Build Settings.
2. Under "Scenes In Build," make sure the most recent scene is the only one checked.
3. Set platform to Universal Windows Platform.
3. Set Target Device to HoloLens.
5. Set Architecture to ARM64.
6. Set Build Type to D3D Project.
7. Press Build. Choose the Builds folder of Visual Cues as the target destination.

## Deploying
1. Connect the HoloLens to your PC via a USB-C cable.
2. After the project builds, open the solution in Visual Studio. (Make sure you have the necessary Visual Studio extensions installed.)
3. Adjust Configuration to Debug.
4. Adjust Platform to ARM64. 
5. Adjust Target to Device.
6. Click "Start Without Debugging" under Debug in the top menu.
7. The first time you connect to the HoloLens, Visual Studio may ask you for a PIN. To get this, go into the HoloLens Settings/Update and Security/For Developers and choose "Pair."
8. Once you hear the bootup sound on the HoloLens, it is safe to disconnect the USB-C cable.

If this doesn't work, I suggest asking on the [HoloDevelopers Slack](https://holodevelopersslack.azurewebsites.net/).

# Experiment Process

Prototype setup and conduct as used in the paper:

1. Set up an obstacle course according to one of the [predetermined layouts](https://imgur.com/a/NXOB8BM). [QR Codes](https://docs.google.com/document/d/1AcF6-MDEuMgz8fVGd6OxW-DgXVH-ZRz1i57YcW5o8Vc/edit?usp=sharing) should go to the left of the starting line at approximately 1.4m. (May need to adjust so wide and low obstacles are correctly placed on the floor.) High obstacles should go at head height for the participant. 
2. Make sure a computer is synced to the HoloLens 2. (See "Remote Operation" below.)
3. Fit HoloLens 2 to participant. Run eye calibration. (A bluetooth keyboard is helpful for this.)
4. Launch AROA app. (This can be done via the Windows Device Portal or by having the participant say "Obstacle Avoidance" while the Start menu is open.)
5. Use the up or down arrows to adjust high obstacles to eye level for the participant.
6. If the participant has vision loss in one eye, adjust the heads-up cues using the Shift HUD Left/Right and Increase/Decrease HUD Size commands.
7. Ask participant to scan QR code for the current course.
8. Ask participant to look forward and close their eyes so they don't see the whole course preview ahead of time.
9. Using Toggle Calibration, Rotate Left, and Rotate Right, adjust the world-locked visual cues to match the real world position of obstacles. You may need to ask the participant to re-scan the QR code.
10. Turn off calibration mode and ensure that the app is set to the correct cue mode. The debug window will show your mode, layout, and direction, and will display "OK TO EXPERIMENT" if calibration mode is off. Make sure to turn off the debug text using the keyboard command 9 before starting.
11. Have the participant stand at the center of the starting line. Count them off, and press 3 for Begin Logging as they start.  When they cross the finish line, press 4 for End Logging. (Note that this will automatically cause direction to shift from Forward to Backward or vice versa for logging and debug purposes.)
12. Repeat the previous step, but starting at the end of the course and going backwards.
13. Adjust the layout and repeat steps 7-12 for each additional trial.

# Remote Operation

## Connecting to the HoloLens

1. Make sure the computer and HoloLens are on the same wifi network.
2. Get the HoloLens’ IPv4 address from Settings/Network and Internet/Properties, e.g. 192.168.1.70
3. In a web browser on the laptop, go to http://[IPv4 address] You should see a “Your connection isn’t private” message. Click “Advanced,” and then “Continue to…”  It will ask you for the username and password you used to connect your HoloLens.
4. You should now see the Windows Device Portal.

## Using the Windows Device Portal

**To view a stream of what the HoloLens sees:**
1. Go to Views / Mixed Reality Capture
2. Under “Capture,” unselect everything except Holograms and PV Camera, and set preview quality to Low
3. Hit “Live Preview.” (Note that there is several seconds of lag, and this may create an additional resource strain on the HoloLens. You may want to disable this preview during experimentation to improve HoloLens performance.)

**To launch an application:**
1. Select it from the dropdown list under “Installed Apps.” (The AROA app is labeled “Obstacle Avoidance”.) 
2. Hit Start.

**To force quit an application:**
1. Go to Views / Apps
1. Under Running Apps, press the X next to the app you want to quit. 
1. Confirm close on the popup.

**To run eye calibration:**
1. Launch the Settings app.
1. Under System / Virtual Input (or using a bluetooth keyboard), type in “Run eye calibration” and hit enter. 
1. You should be able to use Tab and Enter keys to get to the Calibration page, and then select Run Eye Calibration. (It should be the first item selected, so just press enter again.)
Note that you can use shift+enter to move focus backwards.

**To retrieve logged data:**
1. Ensure you are using http:// instead of https:// in your web browser.
1. Ensure “SSL Required” is off under Settings/System/Preferences on the browser.
1. Navigate to System/File Explorer/LocalAppData/Obstacle Avoidance (AROA_…)/LocalState.
1. Click the Save icon next to each file.



## Voice Commands & Keyboard Shortcuts

Voice commands will rarely recognize anyone but the person wearing the HoloLens. Keyboard shortcuts can be used after connecting a bluetooth keyboard to the HoloLens. (We used the [Logitech K380 Multi-Device Bluetooth Keyboard](https://www.logitech.com/en-us/products/keyboards/k380-multi-device.920-007558.html).)

| Command        | Key | Description           
| ------------- |------------- | -----------
| Toggle Collocated Cues | 1 | Turns visibility of world-locked/collocated cues on or off. Default is on.
| Toggle HUD Cues | 2 | Turns visibility of heads-up/HUD cues on or off. Default is on.
| Begin Logging | 3 | Begins logging an obstacle course trial.
| End Logging | 4 | Stops logging an obstacle course trial and saves record.
| Obstacles Front | 8 | Resets obstacles to be in front of user.
| Toggle Debug | 9 | Toggles visibility of debug text. Default is on.
| Shift HUD Right | L | Shifts heads-up cues to the right.
| Shift HUD Left | J | Shifts heads-up cues to the left.
| Increase HUD Size | I | Expands heads-up cues horizontally.
| Decrease HUD Size | K | Contracts heads-up cues horizontally.
| Toggle Calibration | C | Removes distance limit on world-locked obstacles and displays calibration obstacle; sets all heads-up cues to On. Default is on.
| Increase Front Angle | [ | Increases the size of the "Front Angle", enabling heads-up cues to point to obstacles further to the side of the user. Default is 75 degrees with every button press adding 5 degrees, to a maximum of 90 degrees.
| Decrease Front Angle | ] | As above, but decreases by 5 degrees to a minimum of 0 degrees.
| Increase HUD Threshold | ; | Increases the HUD threshold. The higher the number, the more the user will have to look away from the obstacle to activate heads-up cues. Default is 0.15 with every button press adding 0.05 degrees to a maximum of 1.
| Decrease HUD Threshold | ' (apostrophe) | As below, but decreases by 0.05 to a minimum of 0.
| Rotate Left | <- (left arrow) | Rotates obstacles to the left by 1 degree.
| Rotate Right | -> (right arrow) | Rotates obstacles to the right by 1 degree.
| Raise High Obstacles | up arrow | Raises high obstacles by 0.0254m (1 inch). Default height is 60 inches.
| Lower High Obstacles | down arrow | Lowers high obstacles by 0.0254m (1 inch). Default height is 60 inches.
