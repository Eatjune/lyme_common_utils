using System;
using System.Collections.Generic;
using System.Linq;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 关于数组的扩展方法
	/// </summary>
	public static class ArrayExtensions {
		/// <summary>
		/// 数组的2个元素位置调换
		/// </summary>
		public static void Swap<T>(this T[] array, int index1, int index2) {
			(array[index2], array[index1]) = (array[index1], array[index2]);
		}

		/// <summary>
		/// 列表的2个元素位置调换
		/// </summary>
		public static void Swap<T>(this List<T> list, int index1, int index2) {
			(list[index2], list[index1]) = (list[index1], list[index2]);
		}

		/// <summary>
		/// 乱序排序数组
		/// </summary>
		/// <param name="array">数组</param>
		/// <param name="customRandom">自定义随机数</param>
		public static T[] SortRandom<T>(this T[] array, Random customRandom = null) {
			if (customRandom == null) {
				var random = new System.Random();
				return array.OrderBy(n => random.Next()).ToArray();
			} else {
				return array.OrderBy(n => customRandom.Next()).ToArray();
			}
		}

		/// <summary>
		///  乱序排序列表
		/// </summary>
		/// <param name="list">列表</param>
		/// <param name="customRandom">自定义随机数</param>
		public static List<T> SortRandom<T>(this List<T> list, Random customRandom = null) {
			if (customRandom == null) {
				var random = new System.Random();
				return list.OrderBy(n => random.Next()).ToList();
			} else {
				return list.OrderBy(n => customRandom.Next()).ToList();
			}
		}
	}
}