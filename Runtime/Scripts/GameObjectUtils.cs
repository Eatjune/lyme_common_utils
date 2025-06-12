namespace LymeGame.Utils.Common {
	using System;
	using UnityEngine;

	public static class GameObjectUtils {
		/// <summary>
		/// 根据类名生成GameObject并附加该类实例
		/// </summary>
		/// <param name="className">类名</param>
		public static GameObject CreateObjectByString(string className) {
			Type _passiveType = Type.GetType(className);
			if (_passiveType == null) return null;
			var _passiveSkillObject = new GameObject(className);
			_passiveSkillObject.AddComponent(_passiveType);
			return _passiveSkillObject;
		}

		/// <summary>
		/// 获取或增加组件
		/// </summary>
		public static T GetOrAddComponent<T>(this UnityEngine.Object uo) where T : Component {
			return uo.GetComponent<T>() ?? uo.AddComponent<T>();
		}

		public static T GetComponent<T>(this UnityEngine.Object uo) {
			if (uo is GameObject) {
				return ((GameObject) uo).GetComponent<T>();
			} else if (uo is Component) {
				return ((Component) uo).GetComponent<T>();
			} else {
				throw new NotSupportedException();
			}
		}

		public static T AddComponent<T>(this UnityEngine.Object uo) where T : Component {
			if (uo is GameObject) {
				return ((GameObject) uo).AddComponent<T>();
			} else if (uo is Component) {
				return ((Component) uo).gameObject.AddComponent<T>();
			} else {
				throw new NotSupportedException();
			}
		}
	}
}