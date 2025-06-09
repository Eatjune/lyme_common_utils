using System;

namespace LymeGame.Utils.Common {
	public static class TimeUtils {
		private static readonly long Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

		// 时间戳参考文档：https://blog.csdn.net/weixin_45023328/article/details/120545791
		// 时间戳Tick 微秒 纳秒 毫秒 秒 转换：https://blog.csdn.net/lishiyuzuji/article/details/7087214
		/// <summary>
		/// 当前时间戳（毫秒)
		/// </summary>
		/// <returns></returns>
		public static long Now() {
			return (DateTime.UtcNow.Ticks - Epoch) / 10000;
		}

		/// <summary>
		/// 13位时间戳转 日期格式   1652338858000 -> 2022-05-12 03:00:58
		/// </summary>
		public static DateTime GetDateTimeMilliseconds(long timestamp) {
			long begtime = timestamp * 10000;
			DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
			long tricks_1970 = dt_1970.Ticks; //1970年1月1日刻度
			long time_tricks = tricks_1970 + begtime; //日志日期刻度
			DateTime dt = new DateTime(time_tricks); //转化为DateTime
			return dt;
		}
	}
}