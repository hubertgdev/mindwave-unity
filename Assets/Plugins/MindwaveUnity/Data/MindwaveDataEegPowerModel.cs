///<summary>
///
///		Represents brain waves data from Mindwave stream.
///
///</summary>
[System.Serializable]
public struct MindwaveDataEegPowerModel
{

	#region Attributes

		public int delta;
		public int theta;
		public int lowAlpha;
		public int highAlpha;
		public int lowBeta;
		public int highBeta;
		public int lowGamma;
		public int highGamma;

	#endregion

}