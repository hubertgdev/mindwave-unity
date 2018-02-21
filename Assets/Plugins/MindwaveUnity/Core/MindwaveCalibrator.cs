#region Headers

	using System.Collections.Generic;

	using UnityEngine;

#endregion


///<summary>
/// 
///		This component gets the data sent by a MindwaveController, and can calculate a ratio of all the
///	stored values for a given brainwave type (delta, theta, ...).
///		Note that the calculation is quite heavy, and you should take care to call CalculateRatio() (or
///	the accessors -Ratio) only once per frame.
///
///</summary>
[AddComponentMenu("Scripts/NeuroSky/Mindwave Calibrator")]
public class MindwaveCalibrator : MonoBehaviour
{

	#region Attributes

		// References

		[Header("References")]

		[SerializeField, Tooltip("If the MindwaveController reference is not set, MindwaveCalibrator will try to get the one on this GameObject")]
		private MindwaveController m_Controller = null;

		// Settings

		[Header("Settings")]

		[SerializeField, Range(1, 1000), Tooltip("Defines the number of brainwaves data collected for calculating average")]
		private int m_MaxDataLength = 100;

		// Flow

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
		/// Calculate the ratio of the total values for the given brainwave type.
		/// Note that a coefficient is applied to each value depending on the "poorSignalLevel" value.
		/// If that value is more or equal to 200 (which means there's no signal), the value will be 0.
		/// </summary>
		public float CalculateRatio(Brainwave _BrainwaveType)
		{
			int total = 0;
			int min = -1;
			int max = -1;

			foreach(MindwaveDataModel data in m_MindwaveData)
			{
				int value = 0;

				switch(_BrainwaveType)
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
				total += value * (data.poorSignalLevel / MindwaveHelper.NO_SIGNAL_LEVEL);
				min = (min == -1) ? min : Mathf.Min(min, value);
				max = (max == -1) ? max : Mathf.Max(max, value);
			}

			float average = (total / m_MindwaveData.Count);
			return Mathf.Clamp01((average - min) / (max - min));
		}

	#endregion

	
	#region Protected Methods
	#endregion

	
	#region Private Methods
	#endregion

	
	#region Accessors

		public int DataCount
		{
			get { return m_MindwaveData.Count; }
		}

		public float DeltaRatio
		{
			get { return CalculateRatio(Brainwave.Delta); }
		}

		public float ThetaRatio
		{
			get { return CalculateRatio(Brainwave.Theta); }
		}

		public float LowAlphaRatio
		{
			get { return CalculateRatio(Brainwave.LowAlpha); }
		}

		public float HighAlphaRatio
		{
			get { return CalculateRatio(Brainwave.HighAlpha); }
		}

		public float LowBetaRatio
		{
			get { return CalculateRatio(Brainwave.LowBeta); }
		}

		public float HighBetaRatio
		{
			get { return CalculateRatio(Brainwave.HighBeta); }
		}

		public float LowGammaRatio
		{
			get { return CalculateRatio(Brainwave.LowGamma); }
		}

		public float HighGammaRatio
		{
			get { return CalculateRatio(Brainwave.HighGamma); }
		}

	#endregion

	
	#region Debug & Tests

		#if UNITY_EDITOR
		#endif

	#endregion

}