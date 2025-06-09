using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class SpriteExtensions {
		/// <summary>
		/// 转换Texture2D为Sprite
		/// </summary>
		public static Sprite ToSprite(this Texture2D self) {
			var rect = new Rect(0, 0, self.width, self.height);
			var pivot = Vector2.one * 0.5f;
			var newSprite = Sprite.Create(self, rect, pivot);

			return newSprite;
		}

		/// <summary>
		/// RectTransform适配Texture大小(给定的默认尺寸为最大值）
		/// <example>
		///       1.设置目标image的sprite 
		///       2.获取目标image的rectTransform组件(可选：恢复其sizeDelta大小，该尺寸为最大值)
		///       3.调用该函数
		/// <code>
		/// image.sprite = someSprite;
		/// var rectTransform = image.GetComponent&lt;RectTransform&gt; ();
		/// rectTransform.sizeDelta = new Vector2(最大宽度, 最大高度);
		/// rectTransform.FitRectTransformToTexture(rectTransform, image.sprite.texture);
		/// </code>
		/// </example>
		/// </summary>
		public static void FitSpriteSize(this RectTransform transform, Texture texture) {
			var width = transform.sizeDelta.x;
			var height = transform.sizeDelta.y;
			//根据宽度计算高度
			var p = ((float) texture.width / texture.height);
			//宽比高大
			if (texture.width > texture.height) {
				height = width / p;
			} else {
				width = height * p;
			}

			transform.sizeDelta = new Vector2(width, height);
		}
	}
}