using UnityEngine;

namespace LymeGame.Utils.Common {
	public static class GizmosUtils {
		/// <summary>
		/// 绘制一个2d矩形
		/// </summary>
		/// <param name="pos">矩形锚点位置</param>
		/// <param name="size">矩形尺寸</param>
		/// <param name="angle">矩形角度</param>
		/// <param name="color">矩形线段颜色</param>
		public static void DrawRect(Vector3 pos, Vector2 size, float angle, Color color = default) {
			var v_angle = angle * Mathf.PI / 180;
			var mSin = Mathf.Sin(v_angle);
			var mCos = Mathf.Cos(v_angle);
			var mC = 1 / 2f;
			Vector3 mLU, mRU, mRD, mLD;

			var mWidth = size.x;
			var mHeight = size.y;
			var x_Lu = (-mC * (mWidth * mCos + mHeight * mSin)) + pos.x;
			var y_Lu = (mC * (mHeight * mCos - mWidth * mSin)) + pos.y;
			var x_Ru = (mC * (mWidth * mCos - mHeight * mSin)) + pos.x;
			var y_Ru = (mC * (mWidth * mSin + mHeight * mCos)) + pos.y;
			var x_Rd = (mC * (mWidth * mCos + mHeight * mSin)) + pos.x;
			var y_Rd = (mC * (mWidth * mSin - mHeight * mCos)) + pos.y;
			var x_Ld = (mC * (mHeight * mSin - mWidth * mCos)) + pos.x;
			var y_Ld = (-mC * (mHeight * mCos + mWidth * mSin)) + pos.y;
			mLU = new Vector3(x_Lu, y_Lu);
			mRU = new Vector3(x_Ru, y_Ru);
			mRD = new Vector3(x_Rd, y_Rd);
			mLD = new Vector3(x_Ld, y_Ld);

			Gizmos.color = color;
			Gizmos.DrawLine(mLU, mRU);
			Gizmos.DrawLine(mRU, mRD);
			Gizmos.DrawLine(mRD, mLD);
			Gizmos.DrawLine(mLD, mLU);
		}
	}
}