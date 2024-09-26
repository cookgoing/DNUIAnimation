namespace DNAni
{
	using System;

	public enum DNUILoopType
	{
		None,
		Restart,
		Yoyo,
	}

	public interface IDNUIAnimation
	{
		DNUILoopType LoopType{get; set;}
		bool IsAuto{get; set;}
		float Delay{get; set;}
		float AniLen{get;}

		event Action<bool> OnAniComplete;

		void PlayForward();
		void PlayBackward();
		void Pause();
		void Resume();
	}
}
