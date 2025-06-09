using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework;
using UnityEngine;

namespace LymeGame.Utils.Common {
	public class TimerComponent : PersistentSingleton_Mono<TimerComponent> {
		/// <summary>
		///     暂停的计时器
		/// </summary>
		protected readonly Dictionary<int, PausedTimer> m_PausedTimer = new();

		/// <summary>
		///     根据timer的到期时间存储 对应的 N个timerId
		/// </summary>
		protected readonly MultiMap<long, int> m_TimeId = new();

		/// <summary>
		///     需要执行的 到期时间
		/// </summary>
		protected readonly Queue<long> m_TimeOutTime = new();

		/// <summary>
		///     到期的所有 timerId
		/// </summary>
		protected readonly Queue<int> m_TimeOutTimerIds = new();

		/// <summary>
		///     存储所有的timer
		/// </summary>
		protected readonly Dictionary<int, Timer> m_Timers = new();

		/// <summary>
		///     需要每帧回调的计时器
		/// </summary>
		protected readonly Dictionary<int, Timer> m_UpdateTimer = new();

		/// <summary>
		///     记录最小时间，不用每次都去MultiMap取第一个值
		/// </summary>
		protected long m_MinTime;

		protected virtual void Update() {
			RunUpdateCallBack();
			if (m_TimeId.Count == 0) {
				return;
			}

			var timeNow = TimeUtils.Now();

			if (timeNow < m_MinTime) {
				return;
			}

			foreach (var kv in m_TimeId) {
				var k = kv.Key;
				if (k > timeNow) {
					m_MinTime = k;
					break;
				}

				m_TimeOutTime.Enqueue(k);
			}

			while (m_TimeOutTime.Count > 0) {
				var time = m_TimeOutTime.Dequeue();
				foreach (var timerId in m_TimeId[time]) {
					m_TimeOutTimerIds.Enqueue(timerId);
				}

				m_TimeId.Remove(time);
			}

			while (m_TimeOutTimerIds.Count > 0) {
				var timerId = m_TimeOutTimerIds.Dequeue();

				m_Timers.TryGetValue(timerId, out var timer);
				if (timer == null) {
					continue;
				}

				RunTimer(timer);
			}
		}

		/// <summary>
		///     执行每帧回调
		/// </summary>
		protected void RunUpdateCallBack() {
			if (m_UpdateTimer.Count == 0) {
				return;
			}

			var timeNow = TimeUtils.Now();
			foreach (var timer in m_UpdateTimer.Values) {
				timer.UpdateCallBack?.Invoke(timer.Time + timer.StartTime - timeNow);
			}
		}

		/// <summary>
		///     执行定时器回调
		/// </summary>
		/// <param name="timer">定时器</param>
		protected virtual void RunTimer(Timer timer) {
			switch (timer.TimerType) {
				case TimerType.OnceWait: {
					var tcs = timer.Callback as TaskCompletionSource<bool>;
					RemoveTimer(timer.ID);
					tcs?.SetResult(true);
					break;
				}
				case TimerType.Once: {
					var action = timer.Callback as Action;
					RemoveTimer(timer.ID);
					action?.Invoke();
					break;
				}
				case TimerType.Repeated: {
					var action = timer.Callback as Action;
					var nowTime = TimeUtils.Now();
					var tillTime = nowTime + timer.Time;
					if (timer.RepeatCount == 1) {
						RemoveTimer(timer.ID);
					} else {
						if (timer.RepeatCount > 1) {
							timer.RepeatCount--;
						}

						timer.StartTime = nowTime;
						AddTimer(tillTime, timer.ID);
					}

					action?.Invoke();

					break;
				}
			}
		}

		/// <summary>
		///     添加定时器
		/// </summary>
		/// <param name="tillTime">延时时间</param>
		/// <param name="id">定时器ID</param>
		protected void AddTimer(long tillTime, int id) {
			m_TimeId.Add(tillTime, id);
			if (tillTime < m_MinTime) {
				m_MinTime = tillTime;
			}
		}

		/// <summary>
		///     删除定时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		protected void RemoveTimer(int id) {
			m_Timers.TryGetValue(id, out var timer);
			if (timer == null) {
				Debug.LogError($"删除了不存在的Timer ID:{id}");
				return;
			}

			ReferencePool.Release(timer);
			m_Timers.Remove(id);
			m_UpdateTimer.Remove(id);
			if (m_PausedTimer.ContainsKey(id)) {
				ReferencePool.Release(m_PausedTimer[id]);
				m_PausedTimer.Remove(id);
			}
		}

		/// <summary>
		///     取消计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public virtual void CancelTimer(int id) {
			if (m_PausedTimer.ContainsKey(id)) {
				ReferencePool.Release(m_PausedTimer[id].Timer);
				ReferencePool.Release(m_PausedTimer[id]);
				m_PausedTimer.Remove(id);
				return;
			}

			RemoveTimer(id);
		}

		/// <summary>
		///     查询是否存在计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public virtual bool IsExistTimer(int id) {
			return m_PausedTimer.ContainsKey(id) || m_Timers.ContainsKey(id);
		}

		/// <summary>
		///     暂停计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public virtual void PauseTimer(int id) {
			m_Timers.TryGetValue(id, out var oldTimer);
			if (oldTimer == null) {
				Debug.LogError($"Timer不存在 ID:{id}");
				return;
			}

			m_TimeId.Remove(oldTimer.StartTime + oldTimer.Time, oldTimer.ID);
			m_Timers.Remove(id);
			m_UpdateTimer.Remove(id);
			var timer = PausedTimer.Create(TimeUtils.Now(), oldTimer);
			m_PausedTimer.Add(id, timer);
		}

		/// <summary>
		///     恢复计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public virtual void ResumeTimer(int id) {
			m_PausedTimer.TryGetValue(id, out var timer);
			if (timer == null) {
				Debug.LogError($"Timer不存在 ID:{id}");
				return;
			}

			m_Timers.Add(id, timer.Timer);
			if (timer.Timer.UpdateCallBack != null) {
				m_UpdateTimer.Add(id, timer.Timer);
			}

			var tillTime = TimeUtils.Now() + timer.GetResidueTime();
			timer.Timer.StartTime += TimeUtils.Now() - timer.PausedTime;
			AddTimer(tillTime, timer.Timer.ID);
			ReferencePool.Release(timer);
			m_PausedTimer.Remove(id);
		}

		/// <summary>
		///     修改定时器时间
		/// </summary>
		/// <param name="id">定时器ID</param>
		/// <param name="time">修改时间</param>
		/// <param name="isChangeRepeat">是否修改如果是RepeatTimer每次运行时间</param>
		public virtual void ChangeTime(int id, long time, bool isChangeRepeat = false) {
			m_PausedTimer.TryGetValue(id, out var pausedTimer);
			if (pausedTimer?.Timer != null) {
				pausedTimer.Timer.Time += time;
				return;
			}

			m_Timers.TryGetValue(id, out var oldTimer);
			if (oldTimer == null) {
				Debug.LogError($"Timer不存在 ID:{id}");
			}

			m_TimeId.Remove(oldTimer.StartTime + oldTimer.Time, oldTimer.ID);
			if (oldTimer.TimerType == TimerType.Repeated && !isChangeRepeat) {
				oldTimer.StartTime += time;
			} else {
				oldTimer.Time += time;
			}

			AddTimer(oldTimer.StartTime + oldTimer.Time, oldTimer.ID);
		}

		/// <summary>
		///     添加执行一次的定时器
		/// </summary>
		/// <param name="time">定时时间</param>
		/// <param name="callback">回调函数</param>
		/// <param name="updateCallBack">每帧回调函数</param>
		/// <returns></returns>
		public virtual int AddOnceTimer(long time, Action callback, Action<long> updateCallBack = null) {
			if (time < 0) {
				Debug.LogError($"new once time too small: {time}");
			}

			var nowTime = TimeUtils.Now();
			var timer = Timer.Create(time, nowTime, TimerType.Once, callback, 1, updateCallBack);
			m_Timers.Add(timer.ID, timer);
			if (updateCallBack != null) {
				m_UpdateTimer.Add(timer.ID, timer);
			}

			AddTimer(nowTime + time, timer.ID);
			return timer.ID;
		}

		/// <summary>
		///     添加执行多次的定时器
		/// </summary>
		/// <param name="time">定时时间</param>
		/// <param name="repeatCount">重复次数 (小于等于零 无限次调用） </param>
		/// <param name="callback">回调函数</param>
		/// <param name="updateCallback">每帧回调函数</param>
		/// <returns>定时器 ID</returns>
		/// <exception cref="Exception">定时时间太短 无意义</exception>
		public virtual int AddRepeatedTimer(long time, int repeatCount, Action callback, Action<long> updateCallback = null) {
			if (time < 0) {
				Debug.LogError($"new once time too small: {time}");
			}

			var nowTime = TimeUtils.Now();
			var timer = Timer.Create(time, nowTime, TimerType.Repeated, callback, repeatCount, updateCallback);
			m_Timers.Add(timer.ID, timer);
			if (updateCallback != null) {
				m_UpdateTimer.Add(timer.ID, timer);
			}

			AddTimer(nowTime + time, timer.ID);
			return timer.ID;
		}

		public virtual void AddRepeatedTimer(out int id, long time, int repeatCount, Action callback, Action<long> updateCallback = null) {
			if (time < 0) {
				Debug.LogError($"new once time too small: {time}");
			}

			var nowTime = TimeUtils.Now();
			var timer = Timer.Create(time, nowTime, TimerType.Repeated, callback, repeatCount, updateCallback);
			m_Timers.Add(timer.ID, timer);
			if (updateCallback != null) {
				m_UpdateTimer.Add(timer.ID, timer);
			}

			id = timer.ID;
			AddTimer(nowTime + time, timer.ID);
		}

		/// <summary>
		///     添加帧定时器
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <returns>定时器 ID</returns>
		public virtual int AddFrameTimer(Action callback) {
			var nowTime = TimeUtils.Now();
			var timer = Timer.Create(1, nowTime, TimerType.Once, callback);
			m_Timers.Add(timer.ID, timer);
			AddTimer(nowTime + 1, timer.ID);
			return timer.ID;
		}

		/// <summary>
		///     timer 类型
		/// </summary>
		protected enum TimerType {
			/// <summary>
			///     默认 无
			/// </summary>
			None,

			/// <summary>
			///     等待执行一次
			/// </summary>
			OnceWait,

			/// <summary>
			///     执行一次
			/// </summary>
			Once,

			/// <summary>
			///     重复执行
			/// </summary>
			Repeated
		}

		/// <summary>
		///     定时器
		/// </summary>
		protected class Timer : IReference {
			/// <summary>
			///     自增id
			/// </summary>
			protected static int m_SerialId;

			static Timer() {
				m_SerialId = 0;
			}

			/// <summary>
			///     timer 类型
			/// </summary>
			public TimerType TimerType { get; protected set; }

			/// <summary>
			///     计时结束回调函数
			/// </summary>
			public object Callback { get; protected set; }

			/// <summary>
			///     每帧回调函数 (返回剩余时间)
			/// </summary>
			public Action<long> UpdateCallBack { get; protected set; }

			/// <summary>
			///     时间
			/// </summary>
			public long Time { get; set; }

			/// <summary>
			///     开始时间
			/// </summary>
			public long StartTime { get; set; }

			/// <summary>
			///     开始时间
			/// </summary>
			public int RepeatCount { get; set; }

			/// <summary>
			///     ID
			/// </summary>
			public int ID { get; protected set; }

			public void Clear() {
				ID = -1;
				Time = 0;
				StartTime = 0;
				Callback = null;
				UpdateCallBack = null;
				RepeatCount = 0;
				TimerType = TimerType.None;
			}

			/// <summary>
			///     创建定时器
			/// </summary>
			/// <param name="time">时间</param>
			/// <param name="startTime">开始时间</param>
			/// <param name="timerType">定时器类型</param>
			/// <param name="callback">回调</param>
			/// <param name="repeatCount">调用次数</param>
			/// <param name="updateCallBack">每帧回调</param>
			/// <returns>定时器</returns>
			public static Timer Create(long time, long startTime, TimerType timerType, object callback, int repeatCount = 0, Action<long> updateCallBack = null) {
				var timer = ReferencePool.Acquire<Timer>();
				timer.ID = m_SerialId++;
				timer.Time = time;
				timer.StartTime = startTime;
				timer.TimerType = timerType;
				timer.Callback = callback;
				timer.RepeatCount = repeatCount;
				timer.UpdateCallBack = updateCallBack;
				return timer;
			}
		}

		/// <summary>
		///     暂停的定时器
		/// </summary>
		protected class PausedTimer : IReference {
			/// <summary>
			///     被暂停的定时器
			/// </summary>
			public Timer Timer { get; protected set; }

			/// <summary>
			///     暂停时间
			/// </summary>
			public long PausedTime { get; protected set; }

			public void Clear() {
				Timer = null;
				PausedTime = 0;
			}

			/// <summary>
			///     获取剩余运行时间
			/// </summary>
			/// <returns>剩余运行时间</returns>
			public long GetResidueTime() {
				return Timer.Time + Timer.StartTime - PausedTime;
			}

			/// <summary>
			///     创建定时器
			/// </summary>
			/// <param name="pausedTime">暂停时间</param>
			/// <param name="pauseTimer">暂停的计时器</param>
			/// <returns>暂停的定时器</returns>
			public static PausedTimer Create(long pausedTime, Timer pauseTimer) {
				var timer = ReferencePool.Acquire<PausedTimer>();
				timer.Timer = pauseTimer;
				timer.PausedTime = pausedTime;
				return timer;
			}
		}
	}
}