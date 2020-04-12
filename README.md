# Mindwave Unity

This is a Unity plugin for developping app and games with the *NeuroSky MindwaveMobile*, on the occasion of the [Serious Game Jam](https://www.eventbrite.fr/e/34697918408).

## Compatibility

This library works with Unity 2017.4, but has not been tested with older versions than Unity 2017.2.

## Installation

### 1. Pair the device

First, **pair the Mindwave headset** with your computer through **Bluetooth**. On Windows, go to Settings > Devices and click on "Add a Bluetooth or other device". In the popup, choose "*Bluetooth*", and wait for "*MindwaveMobile*" device to appear. Click on the device, and **pair it using the code `0000`**.

### 2. Install and run [ThinkGear Connector](http://developer.neurosky.com/docs/doku.php?id=thinkgear_connector_tgc)

Then, you need to **install [ThinkGear Connector](http://developer.neurosky.com/docs/doku.php?id=thinkgear_connector_tgc)**. It's a tool developped by NeuroSky that emits data from any of their device to a network socket. So it allows you to get data from the device, just like you get data from a website, using sockets.

### 3. Change the *API Compatibility Level*

* In your Unity Project, go to **Edit > Project Settings > Player**
* Unfold the "**Other Settings**" section
* Under **Configuration**, change the value of **Api Compatibility Level** to "**.NET 4.x**" (and not "**.NET Standard 2.0**")

**Note**: In Unity 2017 or older versions, the **Api Compatibility Level** must be set to "**.NET 2.0**" (and not "**.NET 2.0 Subset**").

### 4. Start using plugin components

Back to the Unity plugin. You can find a folder `/UnityPackages` at the project root. **Import that package** into your Unity project.

This package contains the classes and components of the plugin, a demo scene and a **"ready-to-use" prefab**.

## Documentation

### Guides

* [Getting Started guide](./Documentation/GettingStarted.md)
* [Bomb Game](./Documentation/BombGame.md)
* [Known issues](./Documentation/KnownIssues.md)

### Components

* [MindwaveManager](./Documentation/MindwaveManager.md)
* [MindwaveController](./Documentation/MindwaveController.md)
* [MindwaveCalibrator](./Documentation/MindwaveCalibrator.md)
* [MindwaveHelper](./Documentation/MindwaveHelper.md)
* [MindwaveUI](./Documentation/MindwaveUI.md)

## Dependencies

This library uses [Jayrock](https://github.com/atifaziz/Jayrock), and needs [ThinkGear Connector](http://developer.neurosky.com/docs/doku.php?id=thinkgear_connector_tgc) app running.