namespace DNAni
{
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("DNAni/Scale In")]
	public partial class Base_Common_ScaleInAni : UITweener
	{
	#region fade
		[Range(0f, 1f)] public float from_fade = 1f;
		[Range(0f, 1f)] public float to_fade = 1f;
		public float duration_fade = 1f;
		public AnimationCurve aniCurve_fade = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

		private float factor_fade = 0f;
		private bool mCached = false;
		private UIRect mRect;
		private Material mMat;
		private SpriteRenderer mSr;
		private float amountPerDelta_fade
		{
			get
			{
				if (duration_fade == 0f) return 1000f;

				return Mathf.Abs(1f / duration_fade) * Mathf.Sign(mAmountPerDelta);
			}
		}
		private void Cache ()
		{
			mCached = true;
			mRect = GetComponent<UIRect>();
			mSr = GetComponent<SpriteRenderer>();

			if (mRect == null && mSr == null)
			{
				Renderer ren = GetComponent<Renderer>();
				if (ren != null) mMat = ren.material;
				if (mMat == null) mRect = GetComponentInChildren<UIRect>();
			}
		}
		public float value_fade
		{
			get
			{
				if (!mCached) Cache();
				if (mRect != null) return mRect.alpha;
				if (mSr != null) return mSr.color.a;
				return mMat != null ? mMat.color.a : 1f;
			}
			set
			{
				if (!mCached) Cache();

				if (mRect != null)
				{
					mRect.alpha = value;
				}
				else if (mSr != null)
				{
					Color c = mSr.color;
					c.a = value;
					mSr.color = c;
				}
				else if (mMat != null)
				{
					Color c = mMat.color;
					c.a = value;
					mMat.color = c;
				}
			}
		}
	#endregion

	#region scale
		public Vector3 from_scale = Vector3.one;
		public Vector3 to_scale = Vector3.one;

		public Vector3 value_scale { get => transform.localScale;  set => transform.localScale = value; }
	#endregion

		public override bool DoUpdate (bool fromSequence = false)
		{
			if (LockAni && !fromSequence) return false;

			float delta = ignoreTimeScale && !useFixedUpdate ? Time.unscaledDeltaTime : Time.deltaTime;
			float time = ignoreTimeScale && !useFixedUpdate ? Time.unscaledTime : Time.time;

			if (!mStarted)
			{
				delta = 0;
				mStarted = true;
				mStartTime = time + delay;
			}

			if (time < mStartTime) return false;

			factor_fade += (duration_fade == 0f) ? 1f : amountPerDelta_fade * delta;
			mFactor += (duration == 0f) ? 1f : amountPerDelta * delta;

			if (style == Style.Loop)
			{
				if (factor_fade > 1f)
				{
					factor_fade -= Mathf.Floor(factor_fade);
				}

				if (mFactor > 1f)
				{
					mFactor -= Mathf.Floor(mFactor);
				}
			}
			else if (style == Style.PingPong)
			{
				if (factor_fade > 1f)
				{
					factor_fade = 1f - (factor_fade - Mathf.Floor(factor_fade));
				}
				else if(factor_fade < 0f)
				{
					factor_fade = -factor_fade;
					factor_fade -= Mathf.Floor(factor_fade);
				}

				if (mFactor > 1f)
				{
					mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
					mAmountPerDelta = -mAmountPerDelta;
				}
				else if (mFactor < 0f)
				{
					mFactor = -mFactor;
					mFactor -= Mathf.Floor(mFactor);
					mAmountPerDelta = -mAmountPerDelta;
				}
			}

			if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
			{
				mFactor = Mathf.Clamp01(mFactor);
				Sample(mFactor, true);
				enabled = false;

				if (current != this)
				{
					UITweener before = current;
					current = this;

					if (onFinished != null)
					{
						mTemp = onFinished;
						onFinished = new List<EventDelegate>();

						EventDelegate.Execute(mTemp);

						for (int i = 0; i < mTemp.Count; ++i)
						{
							EventDelegate ed = mTemp[i];
							if (ed != null && !ed.oneShot) EventDelegate.Add(onFinished, ed, ed.oneShot);
						}
						mTemp = null;
					}

					if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
						eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);

					current = before;
				}

				return true;
			}
			else
			{
				Sample(mFactor, false);
				return false;
			}
		}

		public override void Sample (float factor, bool isFinished)
		{
			float UpdateFactor(float mfactor)
			{
				float val = Mathf.Clamp01(mfactor);

				if (method == Method.EaseIn)
				{
					val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
					if (steeperCurves) val *= val;
				}
				else if (method == Method.EaseOut)
				{
					val = Mathf.Sin(0.5f * Mathf.PI * val);

					if (steeperCurves)
					{
						val = 1f - val;
						val = 1f - val * val;
					}
				}
				else if (method == Method.EaseInOut)
				{
					const float pi2 = Mathf.PI * 2f;
					val = val - Mathf.Sin(val * pi2) / pi2;

					if (steeperCurves)
					{
						val = val * 2f - 1f;
						float sign = Mathf.Sign(val);
						val = 1f - Mathf.Abs(val);
						val = 1f - val * val;
						val = sign * val * 0.5f + 0.5f;
					}
				}
				else if (method == Method.BounceIn)
				{
					val = BounceLogic(val);
				}
				else if (method == Method.BounceOut)
				{
					val = 1f - BounceLogic(1f - val);
				}

				return val;
			}

			factor_fade = UpdateFactor(factor_fade);
			factor = UpdateFactor(factor);

			bool isForward = amountPerDelta > 0f;
			float scaleFactor = (animationCurve != null && isForward) ? animationCurve.Evaluate(factor) : factor;

			OnUpdate(scaleFactor, isFinished);
		}

		protected override void OnUpdate(float factor, bool isFinished) 
		{
			float fadeFactor = (aniCurve_fade != null) ? aniCurve_fade.Evaluate(factor_fade) : factor_fade;

			value_fade = Mathf.Lerp(from_fade, to_fade, fadeFactor);
			value_scale = from_scale * (1f - factor) + to_scale * factor;
		}

		public override void ResetToBeginning ()
		{
			mStarted = false;

			bool isBackward = amountPerDelta < 0f;
			mFactor = isBackward ? 1f : 0f;
			factor_fade = isBackward ? 1f : 0f;

			Sample(mFactor, false);
		}
		

		protected override void UpdateAniParam() 
		{
			IsAuto = false;

			from_fade = 0;
			to_fade = 1;
			duration_fade = 0.3f;
			aniCurve_fade = new AnimationCurve();
			aniCurve_fade.AddKey(new Keyframe(0, 0, 2, 2));
			aniCurve_fade.AddKey(new Keyframe(1, 1, 0, 0));

			from_scale = transform.localScale * 0.5f;
			to_scale = transform.localScale;
			duration = 0.3f;
			animationCurve = new AnimationCurve();
			animationCurve.AddKey(AniUtility.CreateKeyFrame(0, 0, 2.522106f, 2.522106f, 0, 0.3333333f));
			animationCurve.AddKey(AniUtility.CreateKeyFrame(0.496892f, 1.253214f, 0.1137532f, 0.1137532f, 0.0496448f, 0.493615f));
			animationCurve.AddKey(AniUtility.CreateKeyFrame(0.6422406f, 0.806123f, 0.03269948f, 0.03269948f, 0.4947832f, 0.4698927f));
			animationCurve.AddKey(AniUtility.CreateKeyFrame(0.7907559f, 1.124313f, -0.01179517f, -0.01179517f, 0.3333333f, 0.2833816f));
			animationCurve.AddKey(AniUtility.CreateKeyFrame(0.9046756f, 0.9167718f, -0.02037314f, -0.02037314f, 0.3333333f, 0.7869032f));
			animationCurve.AddKey(AniUtility.CreateKeyFrame(1, 1, 0.8731046f, 0.8731046f, 0.3333333f, 0));
		}
	}
}