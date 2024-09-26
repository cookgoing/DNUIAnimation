using System;
using UnityEngine;
using DNAni;

public abstract partial class UITweener : IDNUIAnimation
{
	[SerializeField]private bool _isAuto = true;

	public DNUILoopType LoopType 
	{ 
		get
		{
			switch (style)
			{
				case Style.Once: return DNUILoopType.None;
				case Style.Loop: return DNUILoopType.Restart;
				case Style.PingPong: return DNUILoopType.Yoyo;
				default: Debug.LogError($"[error][LoopType]. unmatch type: {style} "); return DNUILoopType.None;
			}
		}
		set
		{
			switch (value)
			{
				case DNUILoopType.None: style = Style.Once; break;
				case DNUILoopType.Restart: style = Style.Loop; break;
				case DNUILoopType.Yoyo: style = Style.PingPong; break;
				default: Debug.LogError($"[error][LoopType]. unmatch type: {value} "); break;
			}
		}
	}
	public float Delay
	{
		get => delay;
		set => delay = value;
	}
	public float AniLen => duration;
	public bool IsAuto
	{
		get => _isAuto;
		set => _isAuto = value;
	}
	
	public event Action<bool> OnAniComplete;

	protected virtual void Awake()
	{
		enabled = IsAuto;
		EventDelegate.Set(onFinished, OnAniCompleteCallBack);
		UpdateAniParam();
	}

	protected void OnValidate() => UpdateAniParam();

	protected virtual void UpdateAniParam() {}
	public void PlayBackward() => PlayReverse();
	public void Pause() => enabled = false;
	public void Resume() => enabled = true;

	private void OnAniCompleteCallBack()
	{
		bool isForward = mAmountPerDelta > 0;

		OnAniComplete?.Invoke(isForward);
	} 

}
