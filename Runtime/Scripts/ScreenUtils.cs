using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class ScreenUtils {
		/// <summary>
		/// 目标位置是否在屏幕内
		/// </summary>
		/// <param name="targetPos">目标位置</param>
		/// <param name="camera">观察摄像机</param>
		/// <param name="offset">距离边界的偏移</param>
		public static bool IsInScreen(Vector3 targetPos, Camera camera = null, Vector2? offset = null) {
			if (camera == null) camera = Camera.main;
			var viewPos = camera.WorldToViewportPoint(targetPos);
			var dir = (camera.transform.position - viewPos).normalized;
			// var dot = Vector3.Dot(camera.transform.forward, dir);
			var _offset = Vector2.zero;
			if (offset != null) _offset = offset.Value;
			return viewPos.x - _offset.x > 0 && viewPos.x + _offset.x < 1 && viewPos.y - _offset.y > 0 && viewPos.y + _offset.y < 1; // && dot > 0
		}
	}
}