#region Headers

	using UnityEngine;
	using UnityEditor;

#endregion


[CustomEditor(typeof(MindwaveCalibrator))]
public class MindwaveCalibratorEditor : Editor
{

	#region Enums & Subclasses

		private struct MindwaveCalibratorModeLabel
		{
			public MindwaveCalibrator.MindwaveCalibratorMode mode;
			public string label;

			public MindwaveCalibratorModeLabel(MindwaveCalibrator.MindwaveCalibratorMode _Mode, string _Label)
			{
				mode = _Mode;
				label = _Label;
			}
		}

	#endregion


	#region Attributes

		// Constants & Statics

		private static readonly MindwaveCalibratorModeLabel[] s_ModeLabels = { };

		// Defines the maximum data that can be collected in auutomatic mode.
		private const int MAX_DATA_LENGTH = 1000;
		// Defines the maximum value for manual calibration.
		private const int MAX_BRAINWAVE_VALUE = 3000000;

		// GUI settings
		private const float SLIDER_MIN_MAX_BREAKPOINT = 380.0f;
		private const float MIN_MAX_SLIDER_LABEL_SIZE = 180.0f;
		private const float MIN_MAX_SLIDER_INPUT_SIZE = 60.0f;
		private const float MIN_MAX_SLIDER_MARGIN = 4.0f;

		// Cache

			// Component

		// The target component of this CustomEditor, converted as MindwaveCalibrator
		private MindwaveCalibrator m_Component = null;

			// Properties

		// Component's properties as SerializedProperty instances
		private SerializedProperty m_MaxDataLengthProperty = null;
		private SerializedProperty m_DeltaWavesProperty = null;
		private SerializedProperty m_ThetaWavesProperty = null;
		private SerializedProperty m_LowAlphaWavesProperty = null;
		private SerializedProperty m_HighAlphaWavesProperty = null;
		private SerializedProperty m_LowBetaWavesProperty = null;
		private SerializedProperty m_HighBetaWavesProperty = null;
		private SerializedProperty m_LowGammaWavesProperty = null;
		private SerializedProperty m_HighGammaWavesProperty = null;

	#endregion


	#region Initialization

		static MindwaveCalibratorEditor()
		{
			// Initialize the modes' label
			s_ModeLabels = new MindwaveCalibratorModeLabel[2]
			{
				new MindwaveCalibratorModeLabel(MindwaveCalibrator.MindwaveCalibratorMode.Automatic, "Automatic"),
				new MindwaveCalibratorModeLabel(MindwaveCalibrator.MindwaveCalibratorMode.Manual, "Manual")
			};
		}

		private void OnEnable()
		{
			m_Component = (target as MindwaveCalibrator);

			// Init serialized properties
			m_MaxDataLengthProperty = serializedObject.FindProperty("m_MaxDataLength");
			m_DeltaWavesProperty = serializedObject.FindProperty("m_DeltaWaves");
			m_ThetaWavesProperty = serializedObject.FindProperty("m_ThetaWaves");
			m_LowAlphaWavesProperty = serializedObject.FindProperty("m_LowAlphaWaves");
			m_HighAlphaWavesProperty = serializedObject.FindProperty("m_HighAlphaWaves");
			m_LowBetaWavesProperty = serializedObject.FindProperty("m_LowBetaWaves");
			m_HighBetaWavesProperty = serializedObject.FindProperty("m_HighBetaWaves");
			m_LowGammaWavesProperty = serializedObject.FindProperty("m_LowGammaWaves");
			m_HighGammaWavesProperty = serializedObject.FindProperty("m_HighGammaWaves");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			// Draw settings title
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			// Draw Data Mode tabs
			DrawDataModeGUI();
			EditorGUILayout.Space();
			
			switch(m_Component.Mode)
			{
				case MindwaveCalibrator.MindwaveCalibratorMode.Automatic:
					DrawAutomaticDataModeGUI();
					break;

				case MindwaveCalibrator.MindwaveCalibratorMode.Manual:
					DrawManualDataModeGUI();
					break;

				default:
					EditorGUILayout.HelpBox("No GUI defined for this Data Mode setting.", MessageType.Error);
					break;
			}
		}

	#endregion


	#region Public Methods
	#endregion


	#region Private Methods
		
		/// <summary>
		/// Draw tabs for defining the data collecting mode.
		/// </summary>
		private void DrawDataModeGUI()
		{
			int dataMode = GUILayout.Toolbar(FindSelectedModeIndex(m_Component.Mode), GetModeLabels());
			m_Component.Mode = FindModeByIndex(dataMode);
		}

		/// <summary>
		/// Draws the input for setting the number of data to collect.
		/// </summary>
		private void DrawAutomaticDataModeGUI()
		{
			EditorGUILayout.HelpBox("In automatic mode, the Calibrator collects the data sent by the MindwaveController along time, and evaluate a ratio for a given value, using the min and max collected brainwave values.", MessageType.Info);
			EditorGUILayout.Space();

			m_MaxDataLengthProperty.intValue = (int)EditorGUILayout.Slider("Max Collected Data", m_MaxDataLengthProperty.intValue, 0.0f, MAX_DATA_LENGTH);
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Draws the inputs for setting the calibration for each brainwave type.
		/// </summary>
		private void DrawManualDataModeGUI()
		{
			EditorGUILayout.HelpBox("In manual mode, the Calibrator evaluates ratios using user-defined min and max collected brainwave values.", MessageType.Info);
			EditorGUILayout.Space();

			// Draw min/max inputs for each brainwave
			DrawMinMaxSlider("Delta Waves min/max", m_DeltaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("Theta Waves min/max", m_ThetaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("Low Alpha Waves min/max", m_LowAlphaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("High Alpha Waves min/max", m_HighAlphaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("Low Beta Waves min/max", m_LowBetaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("High Beta Waves min/max", m_HighBetaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("Low Gamma Waves min/max", m_LowGammaWavesProperty, 0, MAX_BRAINWAVE_VALUE);
			DrawMinMaxSlider("High Gamma Waves min/max", m_HighGammaWavesProperty, 0, MAX_BRAINWAVE_VALUE);

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawMinMaxSlider(string _Label, SerializedProperty _Property, float _MinLimit, float _MaxLimit)
		{
			float min = _Property.vector2Value.x;
			float max = _Property.vector2Value.y;

			// Get the size of the next property field
			Rect controlRect = EditorGUILayout.GetControlRect(true);

			// If the inspector is too small
			if (controlRect.size.x <= SLIDER_MIN_MAX_BREAKPOINT)
			{
				DrawSmallMinMaxSlider(controlRect, _Label, ref min, ref max, _MinLimit, _MaxLimit);
			}

			else
			{
				DrawLargeMinMaxSlider(controlRect, _Label, ref min, ref max, _MinLimit, _MaxLimit);
			}

			_Property.vector2Value = new Vector2((int)min, (int)max);
		}

		/// <summary>
		/// Draws 2 float input fields for setting min and max.
		/// </summary>
		private void DrawSmallMinMaxSlider(Rect _ControlRect, string _Label, ref float _Min, ref float _Max, float _MinLimit, float _MaxLimit)
		{
			Vector2 pos = _ControlRect.position;
			Vector2 size = _ControlRect.size;

			// Draw label
			size.x = MIN_MAX_SLIDER_LABEL_SIZE;
			EditorGUI.LabelField(new Rect(pos, size), _Label);

			// Draw left (min) input
			pos.x += size.x;
			size.x = (_ControlRect.size.x - (pos.x + MIN_MAX_SLIDER_MARGIN)) / 2;
			_Min = EditorGUI.FloatField(new Rect(pos, size), _Min);

			// Draw left (max) input
			pos.x += size.x + MIN_MAX_SLIDER_MARGIN;
			_Max = EditorGUI.FloatField(new Rect(pos, size), _Max);
		}

		/// <summary>
		/// Draws a MinMaxRange and 2 float fields for setting min and max.
		/// </summary>
		private void DrawLargeMinMaxSlider(Rect _ControlRect, string _Label, ref float _Min, ref float _Max, float _MinLimit, float _MaxLimit)
		{
			Vector2 pos = _ControlRect.position;
			Vector2 size = _ControlRect.size;

			// Draw label
			size.x = MIN_MAX_SLIDER_LABEL_SIZE;
			EditorGUI.LabelField(new Rect(pos, size), _Label);

			// Draw left (min) input
			pos.x += size.x;
			size.x = MIN_MAX_SLIDER_INPUT_SIZE;
			_Min = EditorGUI.FloatField(new Rect(pos, size), _Min);

			// Draw min/max slider
			pos.x += size.x + MIN_MAX_SLIDER_MARGIN;
			size.x = _ControlRect.size.x - (MIN_MAX_SLIDER_LABEL_SIZE + MIN_MAX_SLIDER_INPUT_SIZE * 2 + MIN_MAX_SLIDER_MARGIN * 2);
			EditorGUI.MinMaxSlider(new Rect(pos, size), ref _Min, ref _Max, _MinLimit, _MaxLimit);

			// Draw right (max) input
			pos.x += size.x + MIN_MAX_SLIDER_MARGIN;
			size.x = MIN_MAX_SLIDER_INPUT_SIZE;
			_Max = EditorGUI.FloatField(new Rect(pos, size), _Max);
		}

		/// <summary>
		/// Gets the index of the given mode.
		/// </summary>
		private int FindSelectedModeIndex(MindwaveCalibrator.MindwaveCalibratorMode _Mode)
		{
			int count = s_ModeLabels.Length;
			for(int i = 0; i < count; i++)
			{
				if(s_ModeLabels[i].mode == _Mode)
				{
					return i;
				}
			}

			return 0;
		}

		/// <summary>
		/// Gets the mode at the given index.
		/// </summary>
		private MindwaveCalibrator.MindwaveCalibratorMode FindModeByIndex(int _Index)
		{
			if(_Index >= 0 && _Index < s_ModeLabels.Length)
			{
				return s_ModeLabels[_Index].mode;
			}

			return MindwaveCalibrator.MindwaveCalibratorMode.Automatic;
		}

	#endregion


	#region Accessors

		/// <summary>
		/// Gets the modes' label as a strings array.
		/// </summary>
		private string[] GetModeLabels()
		{
			int count = s_ModeLabels.Length;
			string[] modeLabels = new string[count];

			for(int i = 0; i < count; i++)
			{
				modeLabels[i] = s_ModeLabels[i].label;
			}

			return modeLabels;
		}

	#endregion

}