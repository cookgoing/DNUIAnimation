namespace DNAni
{
	using System.Collections.Generic;
	using UnityEngine;

	public static class AniUtility
	{
		public static void PrintAniCurve(AnimationCurve animationCurve)
		{
			for (int i = 0; i < animationCurve.length; ++i)
			{
				Keyframe frame = animationCurve[i];

				Debug.Log($"frame[{i}]. time: {frame.time}; value: {frame.value}; inTangent: {frame.inTangent}; outTangent: {frame.outTangent}; inWeight: {frame.inWeight}; outWeight: {frame.outWeight}; weightedMode: {frame.weightedMode};");
			}
		}

		public static Keyframe CreateKeyFrame(float time, float value, float inTangent, float outTangent, float inWeight, float outWeight, WeightedMode mode = WeightedMode.None)
		{
			return new Keyframe()
			{
				time = time,
				value = value,
				inTangent = inTangent,
				outTangent = outTangent,
				inWeight = inWeight,
				outWeight = outWeight,
				weightedMode = mode
			};
		}

		public static bool SafeDequeue<T>(Queue<T> queue, out T value)
		{
			if (queue?.Count > 0)
			{
				value = queue.Dequeue();
				return true;
			}

			value = default;
			return false;
		}

		public static bool SafePeek<T>(Queue<T> queue, out T value)
		{
			if (queue?.Count > 0)
			{
				value = queue.Peek();
				return true;
			}

			value = default;
			return false;
		}
	
		public static Vector2 Point2Dto3D(Camera cam2d, Vector2 pointIn2D, float pointZInWorld)
		{
			// 想象一个3d（FOV = 60）的相机，某个特定的位置上，2D相机的画布，刚好填充3D相机视锥的整个面。
			float size2D = cam2d.orthographicSize;
			float scaleFactor = size2D / Mathf.Tan(Mathf.Deg2Rad* 30);
			float cam3DPosZ = -scaleFactor;

			float pointZIn3D = pointZInWorld - cam3DPosZ;
			bool notRender = pointZIn3D <= 0;
			if (notRender) return Vector3.zero;

			float scale = 1 / (pointZIn3D / scaleFactor);
			Vector2 newPos = (Vector2)pointIn2D * scale;

			return new Vector3(newPos.x, newPos.y);
		}
	}
}