# Mindwave Unity - MindwaveManager

The `MindwaveManager` is just a shortcut for working with other components. It's a singleton, that requires `MindwaveController` and `MindwaveCalibrator` components.

It references these two components and provides two accessors to get them : `Controller` and `Calibrator` (these components are detailed below). As a singleton, you can access it from anywhere in your scripts, and it's garanteed that there's constantly only one instance of this component.

#### Accessors

##### `public MindwaveController Controller`

##### `public MindwaveCalibrator Calibrator`