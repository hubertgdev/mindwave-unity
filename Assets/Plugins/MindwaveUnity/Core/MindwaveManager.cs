#region Headers

	using UnityEngine;

#endregion


///<summary>
/// 
///		This component is just a shortcut for working with the Mindwave.
///		It's a Singleton, so you can access to it from anywhere, and it ensures that only one
///	instance is running during the app lifetime.
///	
///		From this component, you can access to the MindwaveController, and the MindwaveCalibrator.
///	The controller connects to the Mindwave and generate event when it send datas. The calibrator
///	can help you to work with brainwaves values by calculating ratios.
///
///</summary>
[AddComponentMenu("Scripts/MindwaveUnity/Mindwave Manager")]
[RequireComponent(typeof(MindwaveController))]
[RequireComponent(typeof(MindwaveCalibrator))]
public class MindwaveManager : MuffinTools.MonoSingleton<MindwaveManager>
{

	#region Attributes

		// References

		private MindwaveController m_Controller = null;
		private MindwaveCalibrator m_Calibrator = null;

	#endregion

	
	#region Protected Methods

		protected override void OnInstanceInit()
		{
			base.OnInstanceInit();

			DontDestroyOnLoad(gameObject);

			m_Controller = GetComponent<MindwaveController>();
			m_Calibrator = GetComponent<MindwaveCalibrator>();
		}

	#endregion

	
	#region Accessors

		public MindwaveController Controller
		{
			get { return m_Controller; }
		}

		public MindwaveCalibrator Calibrator
		{
			get { return m_Calibrator; }
		}

	#endregion

}