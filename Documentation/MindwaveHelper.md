# Mindwave Unity - MindwaveHelper

This static class is a helper for working with the Mindwave device values.

## Scripting

### Constants

```csharp
public const int SENSE_MAX = 100
```

Corresponds to the maximum "eSense" value (meditation or attention).

---

```csharp
public const int BLINK_MAX = 200
```

Corresponds to the maximum blink strength value that the headset can detect.

---

```csharp
public const int NO_SIGNAL_LEVEL = 200
```

Corresponds to the maximum value of "poorSignalLevel" that [ThinkGear Connector](http://developer.neurosky.com/docs/doku.php?id=thinkgear_connector_tgc) can send, meaning that there's no signal to the headset.

### Methods

```csharp
public static int GetSenseRatio(int _SenseValue)
```

Calculates a ratio of a given sense value (meditation or attention).

---

```csharp
public static int GetBlinkRatio(int _BlinkValue)
```

Calculates the ratio of the given blink strength value, on the maximum.