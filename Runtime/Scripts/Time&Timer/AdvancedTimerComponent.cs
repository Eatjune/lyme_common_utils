using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 定时器
	/// </summary>
	public class TimerSingleton : TimerComponent {
		/// <summary>
		///     根据timer的到期游戏（缩放）时间存储 对应的 N个timerId
		/// </summary>
		protected readonly MultiMap<long, int> m_ScaleTimeId = new();

		/// <summary>
		///     需要执行的 到期游戏（缩放）时间
		/// </summary>
		protected readonly Queue<long> m_ScaleTimeOutTime = new();

		/// <summary>
		///     到期的所有游戏（缩放） timerId
		/// </summary>
		protected readonly Queue<int> m_ScaleTimeOutTimerIds = new();

		/// <summary>
		///     存储所有的游戏（缩放）timer
		/// </summary>
		protected readonly Dictionary<int, Timer> m_ScaleTimers = new();

		/// <summary>
		///     记录游戏（缩放）最小时间，不用每次都去MultiMap取第一个值
		/// </summary>
		protected long m_ScaleMinTime;

		/// <summary>
		///     游戏开始时间戳（毫秒)
		/// </summary>
		public long GameStartTime { get; private set; }

		#region 覆盖单例

		public new static bool HasInstance => _instance != null;

		protected new static Common.TimerSingleton _instance;
		protected new bool _enabled;

		/// <summary>
		/// 自动创建
		/// </summary>
		public new static Common.TimerSingleton Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<Common.TimerSingleton>();
					if (_instance == null) {
						GameObject obj = new GameObject();
						obj.name = typeof(Common.TimerSingleton).Name + "_AutoCreated";
						_instance = obj.AddComponent<Common.TimerSingleton>();
					}
				}

				return _instance;
			}
		}

		/// <summary>
		/// 不自动创建
		/// </summary>
		public new static Common.TimerSingleton Current => _instance;

		/// <summary>
		/// 超时时间，距离游戏开始时的时间
		/// </summary>
		public static int OVERTIME_SINCE_START = 10;

		protected override void Awake() {
			InitializeSingleton();
			GameStartTime = TimeUtils.Now();
			Debug.Log($"游戏开始时间:{TimeUtils.GetDateTimeMilliseconds(GameStartTime)},时间戳:{GameStartTime}");
			///检查是否超时,定时器单例开的太晚（想要游戏开始时就开启定时器的话）
			if (Time.realtimeSinceStartup >= OVERTIME_SINCE_START) {
				Debug.LogError($"定时器开的太晚，超过游戏开始后10s，如果需要，请提前加入场景");
			}
		}

		protected override void InitializeSingleton() {
			if (!Application.isPlaying) {
				return;
			}

			if (AutomaticallyUnparentOnAwake) {
				this.transform.SetParent(null);
			}

			if (_instance == null) {
				_instance = this as Common.TimerSingleton;
				DontDestroyOnLoad(transform.gameObject);
				_enabled = true;
			} else {
				if (this != _instance) {
					Destroy(this.gameObject);
				}
			}
		}

		#endregion

		protected override void Update() {
			RunUpdateCallBack();

			//真实时间
			if (m_TimeId.Count > 0) {
				var timeNow = TimeUtils.Now();

				if (timeNow > m_MinTime) {
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

						RunTimer(timer, true);
					}
				}
			}

			//游戏时间（可缩放）
			if (m_ScaleTimeId.Count > 0) {
				//转换成毫秒
				var gameTimeNow = (long) (Time.time * 1000) + GameStartTime;
				if (gameTimeNow > m_ScaleMinTime) {
					foreach (var kv in m_ScaleTimeId) {
						var k = kv.Key;
						if (k > gameTimeNow) {
							m_ScaleMinTime = k;
							break;
						}

						m_ScaleTimeOutTime.Enqueue(k);
					}

					while (m_ScaleTimeOutTime.Count > 0) {
						var time = m_ScaleTimeOutTime.Dequeue();
						foreach (var timerId in m_ScaleTimeId[time]) {
							m_ScaleTimeOutTimerIds.Enqueue(timerId);
						}

						m_ScaleTimeId.Remove(time);
					}

					while (m_ScaleTimeOutTimerIds.Count > 0) {
						var timerId = m_ScaleTimeOutTimerIds.Dequeue();

						m_Timers.TryGetValue(timerId, out var timer);
						if (timer == null) {
							continue;
						}

						RunTimer(timer, false);
					}
				}
			}
		}

		/// <summary>
		/// 添加执行一次的定时器
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="time">定时时间</param>
		/// <param name="realTime">是否是真实（非缩放）时间</param>
		public int SetTimeout(Action callback, float time, bool realTime = false) {
			if (time < 0) {
				Debug.LogError($"new once time too small: {time}");
			}

			var longTime = (long) (time * 1000);
			var nowTime = TimeUtils.Now();
			if (!realTime) {
				nowTime = (long) (Time.time * 1000) + GameStartTime;
			}

			var timer = Timer.Create(longTime, nowTime, TimerType.Once, callback, 1);
			m_Timers.Add(timer.ID, timer);

			if (realTime) {
				AddTimer(nowTime + longTime, timer.ID);
			} else {
				AddScaleTimer(nowTime + longTime, timer.ID);
			}

			return timer.ID;
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
		public int SetInterval(Action callback, float time, int repeatCount = 0, bool realTime = false) {
			if (time < 0) {
				Debug.LogError($"new once time too small: {time}");
			}

			var longTime = (long) (time * 1000);
			var nowTime = TimeUtils.Now();
			if (!realTime) {
				nowTime = (long) (Time.time * 1000) + GameStartTime;
			}

			var timer = Timer.Create(longTime, nowTime, TimerType.Repeated, callback, repeatCount);
			m_Timers.Add(timer.ID, timer);

			if (realTime) {
				AddTimer(nowTime + longTime, timer.ID);
			} else {
				AddScaleTimer(nowTime + longTime, timer.ID);
			}

			return timer.ID;
		}

		/// <summary>
		///     暂停计时器
		/// </summary>
		/// <param name="id">定时器ID</param>
		public override void PauseTimer(int id) {
			m_Timers.TryGetValue(id, out var oldTimer);
			if (oldTimer == null) {
				Debug.LogError($"Timer不存在 ID:{id}");
				return;
			}

			if (m_TimeId.Contains(oldTimer.StartTime + oldTimer.Time, oldTimer.ID)) {
				m_TimeId.Remove(oldTimer.StartTime + oldTimer.Time, oldTimer.ID);
			} else {
				m_ScaleTimeId.Remove(oldTimer.StartTime + oldTimer.Time, oldTimer.ID);
			}

			m_Timers.Remove(id);
			m_UpdateTimer.Remove(id);
			var timer = PausedTimer.Create(TimeUtils.Now(), oldTimer);
			m_PausedTimer.Add(id, timer);
		}

		/// <summary>
		///     添加缩放定时器
		/// </summary>
		/// <param name="tillTime">延时时间</param>
		/// <param name="id">定时器ID</param>
		protected void AddScaleTimer(long tillTime, int id) {
			m_ScaleTimeId.Add(tillTime, id);
			if (tillTime < m_ScaleMinTime) {
				m_ScaleMinTime = tillTime;
			}
		}

		/// <summary>
		///     执行定时器回调
		/// </summary>
		/// <param name="timer">定时器</param>
		/// <param name="realTime">是否是真实（非缩放）时间</param>
		protected void RunTimer(Timer timer, bool realTime) {
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
					if (!realTime) {
						nowTime = (long) (Time.time * 1000) + GameStartTime;
					}

					var tillTime = nowTime + timer.Time;
					if (timer.RepeatCount == 1) {
						RemoveTimer(timer.ID);
					} else {
						if (timer.RepeatCount > 1) {
							timer.RepeatCount--;
						}

						timer.StartTime = nowTime;
						if (realTime) {
							AddTimer(tillTime, timer.ID);
						} else {
							AddScaleTimer(tillTime, timer.ID);
						}
					}

					action?.Invoke();

					break;
				}
			}
		}
	}
}