namespace LymeGame.Utils.Common {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// 按顺序依次invoke的事件
	/// 数字越大，越迟invoke
	/// </summary>
	public class OrderAction {
		public Dictionary<int, Action> ActionList = new Dictionary<int, Action>();

		/// <summary>
		/// 增加监听
		/// </summary>
		/// <param name="action">注册事件</param>
		/// <param name="order">执行顺序</param>
		public void AddListener(Action action, int order = 100) {
			if (ActionList.ContainsKey(order)) {
				ActionList[order] += action;
			} else {
				ActionList.Add(order, action);
			}
		}

		/// <summary>
		/// 删除监听
		/// </summary>
		/// <param name="action">注册事件</param>
		/// <param name="order">执行顺序</param>
		public void RemoveListener(Action action, int order = 100) {
			if (ActionList.ContainsKey(order)) {
				ActionList[order] -= action;
			}
		}

		public void Invoke() {
			var sortedActionList = ActionList.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			foreach (var action in sortedActionList.Values) {
				action.Invoke();
			}
		}
	}

	/// <summary>
	/// 按顺序依次invoke的事件
	/// 数字越大，越迟invoke
	/// </summary>
	public class OrderAction<T> {
		public Dictionary<int, Action<T>> ActionList = new Dictionary<int, Action<T>>();

		/// <summary>
		/// 增加监听
		/// </summary>
		/// <param name="action">注册事件</param>
		/// <param name="order">执行顺序</param>
		public void AddListener(Action<T> action, int order = 100) {
			if (ActionList.ContainsKey(order)) {
				ActionList[order] += action;
			} else {
				ActionList.Add(order, action);
			}
		}

		/// <summary>
		/// 删除监听
		/// </summary>
		/// <param name="action">注册事件</param>
		/// <param name="order">执行顺序</param>
		public void RemoveListener(Action<T> action, int order = 100) {
			if (ActionList.ContainsKey(order)) {
				ActionList[order] -= action;
			}
		}

		public void Invoke(T obj) {
			var sortedActionList = ActionList.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			foreach (var action in sortedActionList.Values) {
				action.Invoke(obj);
			}
		}

		public void Clear() {
			ActionList.Clear();
		}
	}
}