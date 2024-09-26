namespace DNAni
{
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

	}
}