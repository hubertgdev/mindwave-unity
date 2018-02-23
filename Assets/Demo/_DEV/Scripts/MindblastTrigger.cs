#region Headers

	using System.Collections;

	using UnityEngine;

#endregion


///<summary>
/// 
///		
///
///</summary>
[AddComponentMenu("Scripts/Game/Mindblast Trigger")]
public class MindblastTrigger : MonoBehaviour
{

	private enum BlastPhase
	{
		None,
		Meditation,
		Focus
	}

	#region Attributes

		// Settings

		[Header("Settings")]

		[SerializeField, Range(0.0f, 100.0f)]
		private float m_MinMeditation = 30.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		private float m_MeditationToInitBlast = 80.0f;

		[SerializeField, Range(0.0f, 30.0f)]
		private float m_MeditationPhaseDuration = 5.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		private float m_RequiredAttention = 80.0f;

		[SerializeField, Range(0.0f, 30.0f)]
		private float m_FocusPhaseDuration = 1.0f;

		[SerializeField, Range(0.0f, 60.0f)]
		private float m_BlastReloadDelay = 10.0f;

		// Flow

		private Coroutine m_Routine = null;
		private BlastPhase m_CurrentBlastPhase = BlastPhase.None;

	#endregion

	
	#region Engine Methods

		private void Start()
		{
			MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
		}

	#endregion

	
	#region Public Methods

		public void OnUpdateMindwaveData(MindwaveDataModel _Data)
		{
			if(m_CurrentBlastPhase == BlastPhase.None)
			{
				if(_Data.eSense.meditation >= m_MeditationToInitBlast)
				{
					LoadBlast();
				}
			}

			else if(m_CurrentBlastPhase == BlastPhase.Meditation)
			{
				if (_Data.eSense.meditation <= m_MinMeditation)
				{
					CancelMeditationPhase();
				}
			}

			else if(m_CurrentBlastPhase == BlastPhase.Focus)
			{
				if(_Data.eSense.attention >= m_RequiredAttention)
				{
					TriggerBlast();
				}
			}
		}

	#endregion

	
	#region Protected Methods
	#endregion

	
	#region Private Methods

		/// <summary>
		/// Enters in meditation phase, and start the loading blast trigger timer.
		/// </summary>
		private void LoadBlast()
		{
			m_CurrentBlastPhase = BlastPhase.Meditation;

			CancelRoutine();
			m_Routine = StartCoroutine(EnterMeditationPhase(m_MeditationPhaseDuration, m_FocusPhaseDuration));
		}

		private IEnumerator EnterMeditationPhase(float _MeditationPhaseDuration, float _FocusPhaseDuration)
		{
			OnEnterMeditationPhase();
			yield return new WaitForSeconds(_MeditationPhaseDuration);
			OnExitMeditationPhase();
			m_Routine = StartCoroutine(EnterFocusPhase(_FocusPhaseDuration));
		}

		private IEnumerator EnterFocusPhase(float _FocusPhaseDuration)
		{
			OnEnterFocusPhase();
			yield return new WaitForSeconds(_FocusPhaseDuration);
			OnCancelFocusPhase();
			CancelRoutine();
		}

		private void OnEnterMeditationPhase()
		{
			Debug.Log("Enter Meditation phase");
		}

		private void OnCancelMeditationPhase()
		{
			Debug.Log("Cancel Meditation phase");
		}

		private void OnExitMeditationPhase()
		{
			Debug.Log("Exit Meditation phase");
		}

		private void OnEnterFocusPhase()
		{
			Debug.Log("Enter Focus phase");
		}

		private void OnCancelFocusPhase()
		{
			Debug.Log("Cancel Focus phase");
		}

		private void OnExitFocusPhase()
		{
			Debug.Log("Enter Focus phase");
		}

		private void CancelRoutine()
		{
			if(m_Routine != null)
			{
				StopCoroutine(m_Routine);
				m_Routine = null;
			}
		}

		private void CancelMeditationPhase()
		{
			CancelRoutine();
			m_CurrentBlastPhase = BlastPhase.None;
			OnCancelMeditationPhase();
		}

		private void TriggerBlast()
		{
			CancelRoutine();
			m_CurrentBlastPhase = BlastPhase.None;
			OnExitFocusPhase();
			Debug.Log("Trigger blast!");
		}

	#endregion

	
	#region Accessors
	#endregion

	
	#region Debug & Tests

		#if UNITY_EDITOR
		#endif

	#endregion

}