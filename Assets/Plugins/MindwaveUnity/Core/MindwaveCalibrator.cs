#region Headers

	using System.Collections.Generic;

	using UnityEngine;

#endregion


///<summary>
/// 
///		This component gets the data sent by a MindwaveController, and can calculate a ratio of all the
///	stored values for a given brainwave type (delta, theta, ...).
///		Note that the calculation is quite heavy, and you should take care to call CalculateRatio() only once per frame.
///
///</summary>
[AddComponentMenu("Scripts/MindwaveUnity/Mindwave Calibrator")]
public class MindwaveCalibrator : MonoBehaviour
{

	#region Enums & Subclasses

		/// <summary>
		/// The different calibration data getting modes.
		/// </summary>
		public enum MindwaveCalibratorMode
		{
			Automatic,
			Manual
		}

	#endregion


	#region Attributes

		// References

		[Header("References")]

		[SerializeField]
		[Tooltip("If the MindwaveController reference is not set, MindwaveCalibrator will try to get the one on this GameObject")]
		private MindwaveController m_Controller = null;

		// Settings

		[SerializeField, HideInInspector]
		[Tooltip("If \"Automatic\", the brainwave values will be collected along time. Else, if \"Manual\", you have to define the min and max values for each brainwave")]
		private MindwaveCalibratorMode m_DataMode = MindwaveCalibratorMode.Automatic;

		[SerializeField, HideInInspector]
		[Tooltip("Defines the number of brainwaves data collected for calculating average")]
		private int m_MaxDataLength = 100;

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for Delta brainwaves")]
		private Vector2 m_DeltaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for Theta brainwaves")]
		private Vector2 m_ThetaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for Low Alpha brainwaves")]
		private Vector2 m_LowAlphaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for High Alpha brainwaves")]
		private Vector2 m_HighAlphaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for Low Beta brainwaves")]
		private Vector2 m_LowBetaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for High Beta brainwaves")]
		private Vector2 m_HighBetaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for Low Gamma brainwaves")]
		private Vector2 m_LowGammaWaves = new Vector2(0, 2000000);

		[SerializeField, HideInInspector]
		[Tooltip("Defines the min and max interval for High Gamma brainwaves")]
		private Vector2 m_HighGammaWaves = new Vector2(0, 2000000);

		// Flow

		// The list of collected data along time. Used to calculate the ratio for a given value, using EvaluateRatio().
		private Queue<MindwaveDataModel> m_MindwaveData = new Queue<MindwaveDataModel>();

	#endregion

	
	#region Engine Methods

		private void Awake()
		{
			if (m_Controller == null)
			{
				m_Controller = GetComponent<MindwaveController>();
			}

			if(m_Controller != null)
			{
				m_Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
			}

			else
			{
				Debug.LogWarning("This MindwaveCalibrator has no reference on a MindwaveController");
			}
		}

	#endregion

	
	#region Public Methods

		public void OnUpdateMindwaveData(MindwaveDataModel _Data)
		{
			if(m_MindwaveData.Count >= m_MaxDataLength)
			{
				m_MindwaveData.Dequeue();
			}

			if(m_MindwaveData.Count < m_MaxDataLength)
			{
				m_MindwaveData.Enqueue(_Data);
			}
		}

		/// <summary>
		/// Evaluate the ratio of the given value, according to the average collected value of the
		/// given brainwave.
		/// Note that a coefficient is applied to each value depending on the "poorSignalLevel" value.
		/// If that value is more or equal to 200 (which means there's no signal), the value will be 0.
		/// </summary>
		/// <param name="_Value">The value you want to evaluate.</param>
		public float EvaluateRatio(Brainwave _BrainwaveType, float _Value)
		{
			float ratio = 0.0f;

			switch(m_DataMode)
			{
				case MindwaveCalibratorMode.Automatic:
					ratio = EvaluateRatioAutomatic(_BrainwaveType, _Value);
					break;

				case MindwaveCalibratorMode.Manual:
					ratio = EvaluateRatioManual(_BrainwaveType, _Value);
					break;

				default:
					break;
			}

			return ratio;
		}

	#endregion

	
	#region Private Methods

		/// <summary>
		/// Evaluates the ratio based on defined min/max values for the given brainwave.
		/// </summary>
		private float EvaluateRatioManual(Brainwave _BrainwaveType, float _Value)
		{
			Vector2 minMax = Vector2.zero;

			switch(_BrainwaveType)
			{
				case Brainwave.Delta:
					minMax = m_DeltaWaves;
					break;

				case Brainwave.Theta:
					minMax = m_ThetaWaves;
					break;

				case Brainwave.LowAlpha:
					minMax = m_LowAlphaWaves;
					break;

				case Brainwave.HighAlpha:
					minMax = m_HighAlphaWaves;
					break;

				case Brainwave.LowBeta:
					minMax = m_LowBetaWaves;
					break;

				case Brainwave.HighBeta:
					minMax = m_HighBetaWaves;
					break;

				case Brainwave.LowGamma:
					minMax = m_LowGammaWaves;
					break;

				case Brainwave.HighGamma:
					minMax = m_HighGammaWaves;
					break;
			}

			// On minMax vectors: x = min, y = max
			int diff = (int)(minMax.y - minMax.x);
			return (diff == 0) ? 0 : Mathf.Clamp01((_Value - minMax.x) / diff);
		}

		/// <summary>
		/// Evaluates the ratio based on the collected data.
		/// </summary>
		private float EvaluateRatioAutomatic(Brainwave _BrainwaveType, float _Value)
		{
			int total = 0;
			int min = -1;
			int max = -1;

			foreach (MindwaveDataModel data in m_MindwaveData)
			{
				int value = 0;

				switch (_BrainwaveType)
				{
					case Brainwave.Delta:
					value = data.eegPower.delta;
					break;

					case Brainwave.Theta:
					value = data.eegPower.theta;
					break;

					case Brainwave.LowAlpha:
					value = data.eegPower.lowAlpha;
					break;

					case Brainwave.HighAlpha:
					value = data.eegPower.highAlpha;
					break;

					case Brainwave.LowBeta:
					value = data.eegPower.lowBeta;
					break;

					case Brainwave.HighBeta:
					value = data.eegPower.highBeta;
					break;

					case Brainwave.LowGamma:
					value = data.eegPower.lowGamma;
					break;

					case Brainwave.HighGamma:
					value = data.eegPower.highGamma;
					break;

					default:
					break;
				}

				// Apply a coefficient to the value, depending on the quality of the signal to the Mindwave.
				int signalQuality = (MindwaveHelper.NO_SIGNAL_LEVEL - data.poorSignalLevel);
				total += value * (signalQuality / MindwaveHelper.NO_SIGNAL_LEVEL);
				min = (min == -1) ? value : Mathf.Min(min, value);
				max = (max == -1) ? value : Mathf.Max(max, value);
			}

			int diff = (max - min);
			return (diff == 0) ? 0 : Mathf.Clamp01((_Value - min) / diff);
		}

	#endregion

	
	#region Accessors

		public int DataCount
		{
			get { return m_MindwaveData.Count; }
		}

		public MindwaveCalibratorMode Mode
		{
			get { return m_DataMode; }
			set { m_DataMode = value; }
		}

	#endregion

}