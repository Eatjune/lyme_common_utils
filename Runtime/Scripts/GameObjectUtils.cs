using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LymeGame.Utils.Common {
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
	}
}