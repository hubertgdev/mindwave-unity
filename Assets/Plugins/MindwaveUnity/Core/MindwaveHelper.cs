///<summary>
///
///		Contains helper methods for working with the Neurosky Mindwave values.
///
///</summary>
public static class MindwaveHelper
{

	#region Attributes

		// Constants & Statics
		
		// Maximum "eSense" value (meditaiton or attention)
		public const int SENSE_MAX = 100;

		// Maximum blink strength value
		public const int BLINK_MAX = 200;

		// The maximum value of "poorSignalLevel", meaning that the headset has no signal
		public const int NO_SIGNAL_LEVEL = 200;

	#endregion


	#region Public Methods

		/// <summary>
		/// Calculates a ratio of a given sense value (meditation or attention).
		/// </summary>
		public static float GetSenseRatio(int _SenseValue)
		{
			return (_SenseValue / SENSE_MAX);
		}
		
		/// <summary>
		/// Calculates the ratio of the given blink strength value.
		/// </summary>
		public static float GetBlinkRatio(int _BlinkValue)
		{
			return (_BlinkValue / BLINK_MAX);
		}

	#endregion

}