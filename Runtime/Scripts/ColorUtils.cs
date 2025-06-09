using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class ColorUtils {
		/// <summary>
		/// 将hex转为color类型
		/// </summary>
		public static Color HexToColor(string hex) {
			// 移除可能存在的 # 符号
			if (hex.StartsWith("#")) {
				hex = hex.Substring(1);
			}

			// 确保输入的长度是 6 或 8 位（RGB 或 ARGB）
			if (hex.Length != 6 && hex.Length != 8) {
				Debug.LogError("无效的十六进制颜色格式");
				return Color.black;
			}

			// 将十六进制字符串转换为整数
			uint colorInt;
			if (!uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out colorInt)) {
				Debug.LogError("无法解析十六进制颜色字符串");
				return Color.black;
			}

			// 提取各个颜色分量
			byte r = (byte) ((colorInt >> 24) & 0xFF);
			byte g = (byte) ((colorInt >> 16) & 0xFF);
			byte b = (byte) ((colorInt >> 8) & 0xFF);
			byte a = (byte) (colorInt & 0xFF);

			// 如果是 RGB 格式，则设置默认透明度为 255
			if (hex.Length == 6) {
				a = 255;
				r = (byte) ((colorInt >> 16) & 0xFF);
				g = (byte) ((colorInt >> 8) & 0xFF);
				b = (byte) (colorInt & 0xFF);
			}

			// 归一化到 [0, 1] 范围
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
		}
	}
}