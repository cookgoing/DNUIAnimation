using UnityEngine;

namespace DNAni
{
	public partial class Base_Common_FadeInOutAni : TweenAlpha
	{
		protected override void UpdateAniParam() 
		{
			from = 1;
			to = 0;
			duration = 0.3f;

			animationCurve = new AnimationCurve();
			animationCurve.AddKey(new Keyframe(0, 0, 2, 2, 0, 0));
			animationCurve.AddKey(new Keyframe(1, 1, 0, 0, 0, 0));
		}
	}
}
