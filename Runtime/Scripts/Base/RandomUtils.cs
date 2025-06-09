using System;
using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 随机数
	/// </summary>
	public static class RandomUtils {
		/// <summary>
		/// 获取一个随机的随机数种子
		/// </summary>
		public static long GetRandomSeed() {
			return UnityEngine.Random.Range(0, 99999999);
		}

		/// <summary>
		/// 设置全局随机数种子
		/// </summary>
		public static void SetRandomSeed(int value) {
			UnityEngine.Random.InitState(value);
		}

		/// <summary>
		/// 获取全局随机数小数,不包括最大，包括最小
		/// </summary>
		public static float Range(float min, float max) {
			return UnityEngine.Random.Range(min, max);
		}

		/// <summary>
		/// 获取全局随机数整数,不包括最大，包括最小
		/// </summary>
		public static int Range(int min, int max) {
			return UnityEngine.Random.Range(min, max);
		}
	}
}