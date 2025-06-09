using System;
using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 随机数
	/// </summary>
	public class Random {
		private readonly System.Random m_random;

		public Random(int _seed) {
			m_random = new System.Random(_seed);
		}

		/// <summary>
		/// 返回一个大于等于 0 且小于 Int32.MaxValue（即 2,147,483,647）的随机整数。
		/// </summary>
		public int Next() {
			return m_random.Next();
		}

		/// <summary>
		/// 获取随机数小数,不包括最大，包括最小
		/// </summary>
		public float GetRange(float min, float max) {
			return (float) m_random.NextDouble() * (max - min) + min;
		}

		/// <summary>
		/// 获取随机数整数,不包括最大，包括最小
		/// </summary>
		public int GetRange(int min, int max) {
			return m_random.Next(min, max);
		}

		/** 默认随机数种子 */
		public int Seed {
			get => m_seed;
			set {
				if (m_seed == 0) {
					m_seed = value;
				} else {
					throw new Exception("Random has set!");
				}
			}
		}

		private int m_seed = 0;
	}
}