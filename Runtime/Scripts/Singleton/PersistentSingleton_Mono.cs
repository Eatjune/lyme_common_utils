using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 持久化泛型mono单例
	/// </summary>
	public class PersistentSingleton_Mono<T> : MonoBehaviour where T : Component {
		[Tooltip("自动取消父级")]
		public bool AutomaticallyUnparentOnAwake = true;

		public static bool HasInstance => _instance != null;


		protected static T _instance;
		protected bool _enabled;

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

			if (AutomaticallyUnparentOnAwake) {
				this.transform.SetParent(null);
			}

			if (_instance == null) {
				_instance = this as T;
				DontDestroyOnLoad(transform.gameObject);
				_enabled = true;
			} else {
				if (this != _instance) {
					Destroy(this.gameObject);
				}
			}
		}
	}
}