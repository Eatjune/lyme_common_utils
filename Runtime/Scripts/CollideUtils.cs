using UnityEngine;
using UnityEngine.Internal;

namespace LymeGame.Utils.Common {
	public static class CollideUtils {
		/// <summary>
		/// 获取某个点附近的空碰撞位置
		/// </summary>
		/// <param name="checkPos">检测点位置</param>
		/// <param name="layerMask">目标层</param>
		/// <param name="maxLength">最大范围，圆形</param>
		public static Vector2 CheckEmptyPlace(Vector3 checkPos, [DefaultValue("DefaultRaycastLayers")] int layerMask = 0, float maxLength = 5) {
			return CheckEmptyPlace(checkPos, 1, layerMask, maxLength);
		}

		/// <summary>
		/// 获取某个点x半径内的空碰撞位置
		/// </summary>
		/// <param name="checkPos">检测点位置</param>
		/// <param name="radius">半径</param>
		/// <param name="layerMask">目标层</param>
		/// <param name="maxLength">最大范围，圆形</param>
		public static Vector2 CheckEmptyPlace(Vector3 checkPos, float radius = 1, [DefaultValue("DefaultRaycastLayers")] int layerMask = 0, float maxLength = 5) {
			var checkNum = 8;
			var frequency = 3;
			for (var x = 1; x <= maxLength; x++) {
				// 根据数量决定旋转角度
				for (var i = 0; i < checkNum; i++) {
					// 计算旋转角度
					float angle = 360 / checkNum * i + 90;
					// 使用公式算出坐标
					//x = centerX + radius * cos(angle * 3.14 / 180)
					//y = centerY + radius * sin(angle * 3.14 / 180)
					var _checkPos = checkPos + new Vector3(x * Mathf.Cos(angle * Mathf.PI / 180), x * Mathf.Sin(angle * Mathf.PI / 180), 0);
					var cols = Physics2D.OverlapCircle(_checkPos, radius, layerMask);
					if (!cols) {
						return _checkPos;
					}
				}

				checkNum *= frequency;
			}

			return Vector2.zero;
		}

		/// <summary>
		/// 判断目标点是否包含该层layer
		/// </summary>
		/// <param name="rangeX"></param>
		/// <param name="rangeY"></param>
		/// <param name="layerMask">目标层级,用LayerMask.GetMask不要用NameToLayer那个是单个层级序列</param>
		/// <returns></returns>
		public static Vector2 GetRandomPositionByLayers(Vector3 originPos, Vector2 rangeX, Vector2 rangeY, [DefaultValue("DefaultRaycastLayers")] int layerMask = 0) {
			for (var i = 0; i < 100; i++) {
				var _targetPos = originPos + new Vector3(RandomUtils.Range(rangeX.x, rangeX.y), RandomUtils.Range(rangeY.x, rangeY.y), -100);
				var results = new RaycastHit2D[1];
				Physics2D.RaycastNonAlloc(_targetPos, Vector3.forward, results, Mathf.Infinity, layerMask);
				if (results[0].collider == null) {
					return _targetPos;
				}
			}

			return Vector2.zero;
		}
	}
}