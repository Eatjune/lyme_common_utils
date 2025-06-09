using System.Collections.Generic;
using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class TransformUtils {
		/// <summary>
		/// 围绕某点旋转指定角度
		/// </summary>
		/// <param name="position">自身坐标</param>
		/// <param name="center">旋转中心</param>
		/// <param name="axis">围绕旋转轴</param>
		/// <param name="angle">旋转角度</param>
		public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle) {
			return Quaternion.AngleAxis(angle, axis) * (position - center) + center;
		}
	}
}