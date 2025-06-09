using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 泛型mono单例
	/// </summary>
	public class Singleton_Mono<T> : MonoBehaviour where T : Component {
		protected static T _instance;
		public static bool HasInstance => _instance != null;
		public static T TryGetInstance() => HasInstance ? _instance : null;


		/// <summary>
		/// 自动创建
		/// </summary>
		public static T Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<T>();
					if (_instance == null) {
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}

				return _instance;
			}
		}
		/// <summary>
		/// 不自动创建
		/// </summary>
		public static T Current => _instance;

		protected virtual void Awake() {
			InitializeSingleton();
		}

		protected virtual void InitializeSingleton() {
			if (!Application.isPlaying) {
				return;
			}

			_instance = this as T;
		}
	}
}