#region Headers

	using UnityEngine;
	using UnityEngine.SceneManagement;

#endregion


///<summary>
/// 
///		
///
///</summary>
[AddComponentMenu("Scripts/Game/Lobby UI")]
public class LobbyUI : MonoBehaviour
{

	#region Attributes

		private const string DEFAULT_GAME_SCENE_NAME = "Game_MainScene";

		// Settings

		[Header("Settings")]

		[SerializeField]
		private string m_GameSceneName = DEFAULT_GAME_SCENE_NAME;

	#endregion

	
	#region Engine Methods
	#endregion

	
	#region Public Methods

		public void StartGame()
		{
			SceneManager.LoadScene(m_GameSceneName);
		}

	#endregion

	
	#region Protected Methods
	#endregion

	
	#region Private Methods
	#endregion

	
	#region Accessors
	#endregion

	
	#region Debug & Tests

		#if UNITY_EDITOR
		#endif

	#endregion

}