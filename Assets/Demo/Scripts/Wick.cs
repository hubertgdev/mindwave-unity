#region Headers

using UnityEngine;

#endregion


///<summary>
/// 
///		Represents the bomb's wick.
///		It has a sparklings particle system, and a different sprite depending on the player's meditation.
///
///</summary>
[AddComponentMenu("Scripts/Game/Wick")]
public class Wick : MonoBehaviour
{

    #region Subclasses

    ///<summary>
    ///	Defines the sprite and the ParticleSystem position for a step.
    ///</summary>
    [System.Serializable]
    private struct WickStep
    {
        public Sprite sprite;
        public Vector3 particlesPosition;

        public WickStep(Sprite _Sprite, Vector3 _ParticlesPosition)
        {
            sprite = _Sprite;
            particlesPosition = _ParticlesPosition;
        }
    }

    #endregion

    #region Attributes

    // References

    [SerializeField]
    [Tooltip("Defines the different steps")]
    private WickStep[] m_WickSteps = { };

    [SerializeField]
    [Tooltip("Reference to the Wick renderer")]
    private SpriteRenderer m_WickRenderer = null;

    [SerializeField]
    [Tooltip("Reference to the Wick sparks particles")]
    private ParticleSystem m_Particles = null;

    #endregion


    #region Engine Methods

    private void Awake()
    {
        SetWickStep(0, false);
    }

    #endregion


    #region Public Methods

    /// <summary>
    /// Updates the current wick step depending on the given attention, and the expected value (required attention).
    /// </summary>
    public void UpdateAttention(float _Attention, float _RequiredAttention)
    {
        float step = _RequiredAttention / m_WickSteps.Length;
        if (step != 0.0f)
        {
            int currentStep = Mathf.FloorToInt(_Attention / step);
            currentStep = (currentStep > 0) ? currentStep - 1 : 0;
            SetWickStep(currentStep, true);
        }
    }

    /// <summary>
    /// Enables sparks.
    /// </summary>
    public void Fire()
    {
        SetWickStep(0, true);
    }

    /// <summary>
    /// Resets the Wick steps and turn off the sparks.
    /// </summary>
    public void Cancel()
    {
        SetWickStep(0, false);
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Sets the wick step (sprite and particles position).
    /// </summary>
    private void SetWickStep(int _Index, bool _PlayParticles)
    {
        if (_PlayParticles)
        {
            m_Particles.Play();
        }
        else
        {
            m_Particles.Stop();
        }

        m_WickRenderer.sprite = m_WickSteps[_Index].sprite;
        m_Particles.transform.localPosition = m_WickSteps[_Index].particlesPosition;
    }

    #endregion

}