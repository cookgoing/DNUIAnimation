namespace DNAni
{
	using UnityEngine;
	
	[AddComponentMenu("DNAni/Fade In")]
	public partial class Base_Common_FadeInAni : TweenAlpha
	{
		protected override void UpdateAniParam() 
		{
			from = 0;
			to = 1;
			duration = 0.3f;
			IsAuto = false;

			animationCurve = new AnimationCurve();
			animationCurve.AddKey(new Keyframe(0, 0, 2, 2));
			animationCurve.AddKey(new Keyframe(1, 1, 0, 0));
		}
	}
}
