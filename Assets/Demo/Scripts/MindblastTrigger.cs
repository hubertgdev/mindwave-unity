#region Headers

	using System.Collections;

	using UnityEngine;

#endregion


///<summary>
/// 
///		Binds the Mindwave values with the Bomb renderer.
///		The pattern to trigger the explosion is :
///			- Get meditation over "MeditationToInitBlast"
///			- Hold meditation over "MinMeditation" during "MeditationPhaseDuration"
///			- After that phase, focus, and get attention over "RequiredAttention"
///			- Hold attention during "FocusPhaseDuration"
///			- BOOM!
///			
///		Note that this script use coroutines for each phase timers and callbacks triggering.
///
///</summary>
[AddComponentMenu("Scripts/Game/Mindblast Trigger")]
public class MindblastTrigger : MonoBehaviour
{

	/// <summary>
	/// Names the different phases of the bomb.
	/// </summary>
	public enum BlastPhase
	{
		None,
		Meditation,
		Focus
	}

	#region Attributes

		// References

		// Bomb script allow to make the bomb grow depending on the attention, and make it explode
		private Bomb m_BombRenderer = null;

		// Wick script allow to fire it and change the step
		private Wick m_WickRenderer = null;

		// Settings

		[Header("Settings")]

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("If the meditation goes down this value, cancel the Mindblast")]
		private float m_MinMeditation = 30.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("If the meditation goes over this value, start the \"Meditation phase\"")]
		private float m_MeditationToInitBlast = 70.0f;

		[SerializeField, Range(0.0f, 30.0f)]
		[Tooltip("You must hold meditation value over MinMeditation during this value (in seconds)")]
		private float m_MeditationPhaseDuration = 5.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("If the player is in \"Meditation phase\", it must hold its attention over this value to initialize \"Focus phase\"")]
		private float m_RequiredAttention = 70.0f;

		[SerializeField, Range(0.0f, 30.0f)]
		[Tooltip("The player has to hold its attention over RequiredAttention during this value (in seconds)")]
		private float m_FocusPhaseDuration = 1.0f;

		[SerializeField]
		private float m_BombExplosionAnimationDuration = 1.2f;

		// Flow

		// The current timer coroutine
		private Coroutine m_Routine = null;

		// The current blast phase
		private BlastPhase m_CurrentBlastPhase = BlastPhase.None;

		// Defines if the wick is currently burning (to update it or not)
		private bool m_IsWickBurning = false;

	#endregion

	
	#region Engine Methods

		private void Awake()
		{
			m_WickRenderer = GetComponentInChildren<Wick>();
			m_BombRenderer = GetComponentInChildren<Bomb>();
		}

		private void Start()
		{
			MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;

			// TESTING
			// You can use the "BrainwaveTester" component to test the MindblastTrigger behavior.
			// If you do so, comment the previous line, and uncomment this following one.
			//GetComponentInChildren<BrainwaveTester>().OnUpdateMindwaveData += OnUpdateMindwaveData;
		}

	#endregion

	
	#region Public Methods

		/// <summary>
		/// Called when the MindwaveController sends new values.
		/// </summary>
		public void OnUpdateMindwaveData(MindwaveDataModel _Data)
		{
			// Update bomb renderer (make the bomb grow depending on the meditation value)
			m_BombRenderer.Grow(_Data.eSense.meditation);

			UpdateWickRenderer(_Data.eSense.attention);

			// If the player is not in a specific phase
			if (m_CurrentBlastPhase == BlastPhase.None)
			{	
				// If its meditation is sufficient
				if (_Data.eSense.meditation >= m_MeditationToInitBlast)
				{
					// Start Meditation phase
					LoadBlast();
				}
			}

			// If the player is in Meditation phase
			else if(m_CurrentBlastPhase == BlastPhase.Meditation)
			{
				// If its meditation is too low
				if (_Data.eSense.meditation <= m_MinMeditation)
				{
					// Cancels the meditation phase
					CancelMeditationPhase();
				}
			}

			// If the player is in focus phase
			else if(m_CurrentBlastPhase == BlastPhase.Focus)
			{
				// If the attention is sufficient
				if(_Data.eSense.attention >= m_RequiredAttention)
				{
					// Make the bomb explode
					TriggerBlast();
				}
			}
		}

	#endregion

	
	#region Protected Methods
	#endregion

	
	#region Private Methods

		/// <summary>
		/// Updates the wick renderer depending on the attention value.
		/// </summary>
		private void UpdateWickRenderer(float _Attention)
		{
			if (m_IsWickBurning)
			{
				m_WickRenderer.UpdateAttention(_Attention, m_RequiredAttention);
			}
		}

		/// <summary>
		/// Enters in meditation phase, and start the loading blast trigger timer.
		/// </summary>
		private void LoadBlast()
		{
			m_CurrentBlastPhase = BlastPhase.Meditation;

			CancelRoutine();
			m_Routine = StartCoroutine(EnterMeditationPhase(m_MeditationPhaseDuration, m_FocusPhaseDuration));
		}

		/// <summary>
		/// Coroutine: started when the player enters in Meditation phase.
		/// </summary>
		private IEnumerator EnterMeditationPhase(float _MeditationPhaseDuration, float _FocusPhaseDuration)
		{
			OnEnterMeditationPhase();
			m_CurrentBlastPhase = BlastPhase.Meditation;
			yield return new WaitForSeconds(_MeditationPhaseDuration);
			OnExitMeditationPhase();

			// If the meditation phase hasn't been cancelled, start focus phase.
			m_Routine = StartCoroutine(EnterFocusPhase(_FocusPhaseDuration));
		}

		/// <summary>
		/// Coroutine: started when the player enters in focus phase.
		/// </summary>
		private IEnumerator EnterFocusPhase(float _FocusPhaseDuration)
		{
			OnEnterFocusPhase();
			m_CurrentBlastPhase = BlastPhase.Focus;
			yield return new WaitForSeconds(_FocusPhaseDuration);

			// If the player focused enough to successfully make the bomb explode, this coroutine should have been cancelled before.
			// If not, cancel the focus phase, and reset the mindblast.
			OnCancelFocusPhase();
			CancelRoutine();
		}

		/// <summary>
		/// Called when player enters in meditation phase.
		/// </summary>
		private void OnEnterMeditationPhase()
		{
			Debug.Log("Enter meditation phase");
		}

		// Called when the meditation phase is cancelled (meditation becomes too low)
		private void OnCancelMeditationPhase()
		{
			Debug.Log("Cancel meditation phase");
		}

		/// <summary>
		/// Called when player exits the meditation phase (all is fine, the focus phase should begin)
		/// </summary>
		private void OnExitMeditationPhase()
		{
			Debug.Log("Exit meditation phase");
		}

		/// <summary>
		/// Called when player enters in focus phase.
		/// </summary>
		private void OnEnterFocusPhase()
		{
			m_WickRenderer.Fire();
			m_IsWickBurning = true;
		}

		/// <summary>
		/// Called when the focus phase is cancelled (the player didn't focus enough during the defined delay)
		/// </summary>
		private void OnCancelFocusPhase()
		{
			m_IsWickBurning = false;
			m_WickRenderer.Cancel();
		}

		/// <summary>
		/// Called when player exits the focus phase (all is fine, the bomb can explode, and the mindblast restarts)
		/// </summary>
		private void OnExitFocusPhase()
		{
			m_IsWickBurning = false;
			m_WickRenderer.Cancel();
			m_BombRenderer.Explode();
		}

		/// <summary>
		/// Cancels the current coroutine, and set it to null.
		/// </summary>
		private void CancelRoutine()
		{
			if(m_Routine != null)
			{
				StopCoroutine(m_Routine);
				m_Routine = null;
			}
		}

		/// <summary>
		/// Cancels the meditation phase (reset mindblast)
		/// </summary>
		private void CancelMeditationPhase()
		{
			CancelRoutine();
			m_CurrentBlastPhase = BlastPhase.None;
			OnCancelMeditationPhase();
		}

		/// <summary>
		/// Notify the end of the focus phase, and restart mindblast
		/// </summary>
		private void TriggerBlast()
		{
			CancelRoutine();
			m_CurrentBlastPhase = BlastPhase.None;
			OnExitFocusPhase();
			StartCoroutine(HideWick(m_BombExplosionAnimationDuration));
		}

		/// <summary>
		/// Coroutine: Hides the bomb's wick during its explosion animation
		/// </summary>
		private IEnumerator HideWick(float _Delay)
		{
			m_WickRenderer.gameObject.SetActive(false);
			yield return new WaitForSeconds(_Delay);
			m_WickRenderer.gameObject.SetActive(true);
		}

	#endregion

}