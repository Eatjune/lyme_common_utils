using System;

namespace LymeGame.Utils.Common {
	public static class TimerUtils {
		/// <summary>
		/// 添加执行一次的定时器
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="time">定时时间</param>
		/// <param name="realTime">是否是真实（非缩放）时间</param>
		public static int SetTimeout(Action callback, float time, bool realTime = false) {
			return TimerSingleton.Instance.SetTimeout(callback, time, realTime);
		}

		/// <summary>
		///     添加执行多次的定时器
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="time">定时时间</param>
		/// <param name="repeatCount">重复次数 (小于等于零 无限次调用） </param>
		/// <param name="realTime">是否是真实（非缩放）时间</param>
		/// <returns>定时器 ID</returns>
		/// <exception cref="Exception">定时时间太短 无意义</exception>
		public static int SetInterval(Action callback, float time, int repeatCount = 0, bool realTime = false) {
			return TimerSingleton.Instance.SetInterval(callback, time, repeatCount, realTime);
		}

		/// <summary>
		///     暂停计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public static void PauseTimer(int id) {
			TimerSingleton.Instance.PauseTimer(id);
		}

		/// <summary>
		///     恢复计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public static void ResumeTimer(int id) {
			TimerSingleton.Instance.ResumeTimer(id);
		}

		/// <summary>
		///     查询是否存在计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public static bool IsExistTimer(int id) {
			return TimerSingleton.Instance.IsExistTimer(id);
		}

		/// <summary>
		///     取消计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public static void CancelTimer(int id) {
			TimerSingleton.Instance.CancelTimer(id);
		}
	}
}