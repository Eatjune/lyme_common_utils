using System;
using System.Collections.Generic;
using System.Linq;

namespace LymeGame.Util {
	public static class ArrayUtility {
		/// <summary>
		///     数组的2个元素位置调换
		/// </summary>
		public static void Swap<T>(this T[] array, int index1, int index2) {
			(array[index2], array[index1]) = (array[index1], array[index2]);
		}

		/// <summary>
		///     列表的2个元素位置调换
		/// </summary>
		public static void Swap<T>(this List<T> list, int index1, int index2) {
			(list[index2], list[index1]) = (list[index1], list[index2]);
		}

		/// <summary>
		///     乱序排序数组
		/// </summary>
		public static T[] SortRandom<T>(this T[] array) {
			var random = new Random();
			return array.OrderBy(n => random.Next()).ToArray();
			// for (var i = array.Length - 1; i > 0; i--) {
			// 	var randomIndex = Random.Range(0, i);
			// 	array.Swap(randomIndex, i);
			// }
		}

		/// <summary>
		///     乱序排序列表
		/// </summary>
		public static List<T> SortRandom<T>(this List<T> list) {
			var random = new Random();
			return list.OrderBy(n => random.Next()).ToList();
			// for (var i = list.Count - 1; i > 0; i--) {
			// 	var randomIndex = Random.Range(0, i);
			// 	list.Swap(randomIndex, i);
			// }
		}
	}
}