#region Headers

	using UnityEngine;

#endregion


///<summary>
/// 
///		Represents the bomb.
///		It grows depending on your meditation, and can explode when you're focused enough.
///
///</summary>
[AddComponentMenu("Scripts/Game/Bomb")]
public class Bomb : MonoBehaviour
{

	#region Attributes

		// Constants & Statics

		// Explosion trigger name in the Bomb's animator.
		private const string EXPLODE_TRIGGER = "Explode";

		// References

		private Animator m_Animator = null;

		// Settings

		[Header("Settings")]

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("The minimum bomb local scale")]
		private float m_GrowMin = 1.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("The maximum bomb local scale")]
		private float m_GrowMax = 2.0f;

		[SerializeField, Range(0.0f, 100.0f)]
		[Tooltip("Defines the growing speed")]
		private float m_GrowSpeed = 12.0f;

		// Flow

		// Defines the current scale target (for smooth rescaling animation)
		private float m_ScaleTarget = 0.0f;

	#endregion

	
	#region Engine Methods

		private void Awake()
		{
			m_Animator = GetComponent<Animator>();
			m_ScaleTarget = m_GrowMin;
		}

		private void Update()
		{
			// Rescale the bomb dependig on the ScaleTarget
			Vector3 targetScale = new Vector3(m_ScaleTarget, m_ScaleTarget, 1.0f);
			transform.localScale = Vector3.Lerp(transform.localScale, targetScale, m_GrowSpeed * Time.deltaTime);
		}

	#endregion

	
	#region Public Methods
	
		/// <summary>
		/// Make the bomb grow depending on the given meditation value, and the min and max scale.
		/// </summary>
		public void Grow(float _Meditation)
		{
			_Meditation = Mathf.Clamp01(_Meditation / (float)MindwaveHelper.SENSE_MAX);
			m_ScaleTarget = m_GrowMin + (m_GrowMax - m_GrowMin) * _Meditation;
		}

		/// <summary>
		/// Makes the bomb play the explosion animation.
		/// </summary>
		public void Explode()
		{
			m_Animator.SetTrigger(EXPLODE_TRIGGER);
		}

	#endregion

}