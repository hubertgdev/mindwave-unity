#region Headers

	using System.Collections;

	using UnityEngine;

#endregion


///<summary>
/// 
///		This scripts emulates eSense data sent by the Mindwave headset, following the expected pattern for Mindblast Trigger.
///
///</summary>
[AddComponentMenu("Scripts/Game/Brainwave Tester")]
public class BrainwaveTester : MonoBehaviour
{

	#region Subclasses

		///<summary>
		///	Defines a pattern for a specific phase
		///</summary>
		[System.Serializable]
		private struct ESensePattern
		{
			[Range(0.0f, 100.0f)]
			[Tooltip("The value to reach")]
			public float target;

			[Range(0.0f, 100.0f)]
			[Tooltip("Target value will be reached in that given duration")]
			public float increaseDuration;

			[Range(0.0f, 100.0f)]
			[Tooltip("Defines a value to hold after the phase is ended")]
			public float valueAfterIncreaseEnd;
		}

	#endregion


	#region Attributes

		// Delegates

		// Defines a delegate for OnUpdateMindwaveData event
		public delegate void MindwaveDataDelegate(MindwaveDataModel _Data);

		// Triggered each frame
		public event MindwaveDataDelegate OnUpdateMindwaveData;

		// Settings

		[Header("Settings")]

		[SerializeField]
		[Tooltip("Defines a pattern for meditation phase")]
		private ESensePattern m_MeditationPattern;

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("Defines a \"sleep\" time between meditation and focus phases, before meditation goes to its valueAfterIncreaseEnd")]
		private float m_PauseBetweenPhases = 5.0f;

		[SerializeField]
		[Tooltip("Defines a pattern for attention phase")]
		private ESensePattern m_AttentionPattern;

		// Flow

		// The current meditation value
		private float m_Meditation = 0.0f;
		//The current attention value
		private float m_Attention = 0.0f;

	#endregion

	
	#region Engine Methods

		private void Start()
		{
			StartCoroutine(StartTesting(m_MeditationPattern, m_AttentionPattern));
		}

	#endregion

	
	#region Private Methods

		/// <summary>
		/// Coroutine: Start increase meditation, pause the script, then increase attention.
		/// </summary>
		private IEnumerator StartTesting(ESensePattern _MeditationPattern, ESensePattern _AtrtentionPattern)
		{
			float timer = 0.0f;
			// Increase meditation during the meditation pattern's increaseDuration
			while(timer < _MeditationPattern.increaseDuration)
			{
				m_Meditation = Mathf.Lerp(0.0f, m_MeditationPattern.target, (timer / m_MeditationPattern.increaseDuration));
				m_Meditation = (m_Meditation > 100.0f) ? 100.0f : m_Meditation;

				// Trigger update event
				if(OnUpdateMindwaveData != null)
				{
					OnUpdateMindwaveData(MakeMindwaveData(m_Meditation, m_Attention));
				}

				timer += Time.deltaTime;
				yield return null;
			}

			m_Meditation = m_MeditationPattern.target;
			// Pause current phase
			yield return new WaitForSeconds(m_PauseBetweenPhases);
			// Reset meditation to its valueAfterIncreaseEnd
			m_Meditation = m_MeditationPattern.valueAfterIncreaseEnd;

			timer = 0.0f;
			// Increase attention during the attention pattern's increaseDuration
			while(timer < m_AttentionPattern.increaseDuration)
			{
				m_Attention = Mathf.Lerp(0.0f, m_AttentionPattern.target, (timer / m_AttentionPattern.increaseDuration));
				m_Attention = (m_Attention > 100.0f) ? 100.0f : m_Attention;

				// Trigger update event
				if (OnUpdateMindwaveData != null)
				{
					OnUpdateMindwaveData(MakeMindwaveData(m_Meditation, m_Attention));
				}

				timer += Time.deltaTime;
				yield return null;
			}

			// Reset attention to its valueAfterIncreaseEnd
			m_Attention = m_AttentionPattern.valueAfterIncreaseEnd;
		}

		/// <summary>
		/// Makes an instance of MindwaveDataModel with the given meditation and attention as eSense values.
		/// </summary>
		private MindwaveDataModel MakeMindwaveData(float _Meditation, float _Attention)
		{
			MindwaveDataModel data = new MindwaveDataModel();
			data.eSense.meditation = Mathf.FloorToInt(_Meditation);
			data.eSense.attention = Mathf.FloorToInt(_Attention);

			return data;
		}

	#endregion

}