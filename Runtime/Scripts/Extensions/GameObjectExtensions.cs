using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class GameObjectExtensions {
		/// <summary>
		/// 获取GameObject所在路径
		/// </summary>
		public static string GetPath(this GameObject obj) {
			var path = obj.name;
			Transform target = obj.transform;
			while (target.parent != null) {
				var parent = target.parent;
				path = parent.gameObject.name + "/" + path;
				target = parent;
			}

			path = obj.scene.name + "/" + path;

			return path;
		}

		/// <summary>
		/// 获取所有子对象
		/// </summary>
		public static GameObject[] Children(this GameObject gameObject) {
			var children = new List<GameObject>();
			for (var i = 0; i < gameObject.transform.childCount; i++) {
				children.Add(gameObject.transform.GetChild(i).gameObject);
			}

			return children.ToArray();
		}
	}
}