///<summary>
///
///		Represents datas extracted from Mindwave stream.
///
///</summary>
[System.Serializable]
public struct MindwaveDataModel
{

	#region Attributes

		// Attention & meditation metrics
		public MindwaveDataESenseModel eSense;

		// Brain waves metrics
		public MindwaveDataEegPowerModel eegPower;

		// Mindwave connection status
		public int poorSignalLevel;
		public string status;

	#endregion


	#region Accessors

		/// <summary>
		/// Checks if this data is relative to a "no signal" value (poorSignalLevel too high).
		/// </summary>
		public bool NoSignal
		{
			get { return (poorSignalLevel >= MindwaveHelper.NO_SIGNAL_LEVEL); }
		}

	#endregion

}