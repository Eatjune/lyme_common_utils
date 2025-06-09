using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// debug工具，绘制
	/// </summary>
	public static class Debug_DrawUtils {
		/// <summary>
		/// 绘制一个圆形
		/// </summary>
		public static void DrawCircle(Vector2 pos, float radius, int segments = 30) {
			CircleDebugComponent.Instance.DrawCircle(pos, radius, segments);
		}

		/// <summary>
		/// 绘制一个圆形
		/// </summary>
		public static void DrawCircle(Vector2 pos, float radius, Color color, int segments = 30) {
			CircleDebugComponent.Instance.DrawCircle(pos, radius, color, segments);
		}
	}
}