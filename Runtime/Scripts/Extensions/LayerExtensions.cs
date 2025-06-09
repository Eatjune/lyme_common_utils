using System;
using System.Collections.Generic;
using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class LayerExtensions {
		/// <summary>
		/// 获取LayerMask的所有开启的层
		/// （即二进制内数值为1）
		/// </summary>
		public static int[] GetOpenLayers(this LayerMask layerMask) {
			var tempList = new List<int>();
			var tempStr = Convert.ToString(layerMask.value, 2);
			for (var i = 0; i < tempStr.Length; i++) {
				if (tempStr[i].ToString() == "1") {
					tempList.Add(tempStr.Length - i - 1);
				}
			}

			return tempList.ToArray();
		}
	}
}