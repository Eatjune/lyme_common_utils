namespace LymeGame.Utils.Common {
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// 绘制debug点
	/// </summary>
	public class CircleDebugComponent : PersistentSingleton_Mono<CircleDebugComponent> {
		public List<CircleParam> Params = new List<CircleParam>();

		private void Update() {
			foreach (var circleParam in Params) {
				_DrawCircle(circleParam.center, circleParam.radius, circleParam.color, circleParam.segments);
			}
		}

		private void _DrawCircle(Vector2 pos, float radius, Color color, int segments = 10) {
			// 计算圆周上的各个点的坐标
			var points = new Vector2[segments + 1];
			for (var i = 0; i < segments; i++) {
				var angle = 360f / segments * i;
				points[i] = new Vector2(pos.x + Mathf.Sin(angle) * radius, pos.y + Mathf.Cos(angle) * radius);
			}

			points[segments] = points[0];

			// 使用Debug.DrawLine连接各个点，画出圆圈
			for (var i = 0; i < segments; i++) {
				Debug.DrawLine(points[i], points[i + 1], color, 0f, false);
			}
		}

		public void DrawCircle(Vector2 pos, float radius, int segments = 30) {
			DrawCircle(pos, radius, Color.red, segments);
		}

		public void DrawCircle(Vector2 pos, float radius, Color color, int segments = 30) {
			Params.Add(new CircleParam() {
				center = pos, radius = radius, color = color, segments = segments
			});
		}

		[Serializable]
		public class CircleParam {
			public Vector2 center;
			public float radius;
			public int segments;
			public Color color;
		}
	}
}