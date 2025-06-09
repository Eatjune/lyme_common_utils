using System.Collections.Generic;
using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class TransformExtensions {
		/// <summary>
		/// 获取所有子对象
		/// </summary>
		public static Transform[] Children(this Transform transform) {
			var children = new List<Transform>();
			for (var i = 0; i < transform.childCount; i++) {
				children.Add(transform.GetChild(i));
			}

			return children.ToArray();
		}

		/// <summary>
		/// 获取某点旋转角度后点
		/// </summary>
		/// <param name="position">自身坐标</param>
		/// <param name="center">旋转中心</param>
		/// <param name="axis">围绕旋转轴</param>
		/// <param name="angle">旋转角度</param>
		public static Vector3 GetRotateAngleDirection(this Vector3 position, Vector3 center, Vector3 axis, float angle) {
			return Quaternion.AngleAxis(angle, axis) * (position - center) + center;
		}

		/// <summary>
		/// 二维空间下使 <see cref="Transform" /> 指向指向目标点的算法，使用世界坐标。
		/// </summary>
		/// <param name="transform"><see cref="Transform" /> 对象。</param>
		/// <param name="lookAtPoint2D">要朝向的二维坐标点。</param>
		/// <remarks>假定其 forward 向量为 <see cref="Vector3.up" />。</remarks>
		public static void LookAt2D(this Transform transform, Vector3 lookAtPoint2D, Vector3 forward) {
			Vector3 v = (lookAtPoint2D - transform.position).normalized;
			if (forward == Vector3.forward) transform.forward = v;
			else if (forward == Vector3.right) transform.right = v;
			else if (forward == Vector3.left) transform.right = -v;
			else if (forward == Vector3.up) transform.up = v;
			else if (forward == Vector3.down) transform.up = -v;
		}
	}
}