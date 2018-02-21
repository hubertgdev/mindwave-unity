using UnityEngine;

namespace MuffinTools
{

///<summary>
/// 
///		If you want a MonoSingleton instance not to be destroyed on load, you have to
///	call DontDestroyOnLoad() in an OnInstanceInit() override.
///		You'll also have to destroy the element manually in an OnDetroyMessage() override.
///	That method is called when another Singleton of the same type is in the scene (than,
///	the other instances will be destroyed). Take care to destroy the script (Destroy(this))
///	if you need the Game Object where this script is attached to, or the entire GameObject
///	(Destroy(gameObject)) if nothing else depends of it.
/// 
///</summary>
public class MonoSingleton<T> : MonoBehaviour
	where T : MonoBehaviour
{

	#region Attributes

		// References

		private static T s_Instance = null;

		// Flow

		private static bool s_Initialized = false;

	#endregion

	
	#region Engine Methods

		private void Awake()
		{
			T currentInstance = (this as T);

			if(SetInstance(currentInstance))
			{
				InitInstance(currentInstance);
			}

			OnAwakeMessage();
		}

		private void OnDestroy()
		{
			if(s_Instance == (this as T))
			{
				NotifyInstanceDestroy();
			}

			OnDestroyMessage();
		}

	#endregion

	
	#region Protected Methods

		protected virtual void OnInstanceInit()
		{
			
		}

		protected virtual void OnAwakeMessage()
		{
			
		}

		protected virtual void OnDestroyMessage()
		{

		}

	#endregion

	
	#region Private Methods

		private void InitInstance(T _Instance)
		{
			if (!s_Initialized)
			{
				if(s_Instance == _Instance)
				{
					s_Initialized = true;
					(_Instance as MonoSingleton<T>).OnInstanceInit();
				}
			}
		}

		private void NotifyInstanceDestroy()
		{
			s_Instance		= null;
			s_Initialized	= false;
		}

		private static MonoSingleton<T> CreateInstanceInScene()
		{
			// Use reflection if we are in Editor to set name
			string gameObjectName =
			#if UNITY_EDITOR
			typeof(T).FullName + "_Singleton";
			#else
			"SingletonInstance";
			#endif

			GameObject obj = new GameObject(gameObjectName);

			return obj.AddComponent<T>() as MonoSingleton<T>;
		}
	
	#endregion

	
	#region Accessors

		private bool SetInstance(T _Instance)
		{
			if(s_Instance == null && _Instance != null)
			{
				s_Instance = _Instance;
			}

			if (s_Instance == _Instance)
			{
				InitInstance(_Instance);
				return true;
			}

			Destroy(_Instance);
			return false;
		}

		public static T Instance
		{
			get
			{
				// If the instance has not been set already
				if(s_Instance == null)
				{
					MonoSingleton<T> instanceInScene = FindObjectOfType<MonoSingleton<T>>();
					// If no instance has been found in scene, create one.
					if(instanceInScene == null)
					{
						instanceInScene = CreateInstanceInScene();
					}
					instanceInScene.SetInstance(instanceInScene as T);
				}

				return s_Instance;
			}
		}

	#endregion

}

}