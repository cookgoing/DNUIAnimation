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
	}
}