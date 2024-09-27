namespace DNAni
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

	[AddComponentMenu("DNAni/Ani Sequence")]
	public partial class DNUIAniSequence : UITweener
	{
		[Serializable]
		public struct AniItem
		{
			public float delay;
			public UITweener aniClip;

			public AniItem(float _delay, UITweener _aniClip)
			{
				delay = _delay;
				aniClip = _aniClip;
			}
		}

		public enum Type
		{
			Parallel,
			Queue,
		}

		[SerializeField] private Type sequenceType;
		[SerializeField] private List<AniItem> aniList;
		public List<AniItem> AniList{get => aniList;}
		public Type SequenceType{get => sequenceType;}
		public override float AniLen
		{
			get
			{
				bool haveAni = aniList?.Count > 0;
				if (!haveAni) return 0;

				float aniLen = 0;
				foreach(AniItem aniItem in aniList)
				{
					switch(sequenceType)
					{
						case Type.Queue: 
							aniLen += aniItem.delay;
							aniLen += aniItem.aniClip?.AniLen ?? 0;
						break;
						case Type.Parallel: 
							float newLen = aniItem.delay + aniItem.aniClip?.AniLen ?? 0;

							aniLen = Mathf.Max(aniLen, newLen);
						break;
					}
				}
				return aniLen;
			}
		}
		
		private Queue<AniItem> aniQueue;
		private float waitTime;
		private List<AniItem> aniParallel;
		private Dictionary<AniItem, float> waitTimeDic;

		protected override void Awake ()
		{
			base.Awake();

			bool haveAni = aniList?.Count > 0;
			if (!haveAni) return;

			ModiftListAni();
		}

		protected override void Start ()
		{
			if (!IsAuto) return;

			Play(true);
		}

		public override bool DoUpdate (bool fromSequence = false)
		{
			if (LockAni && !fromSequence) return false;
			
			bool sequenceFinished = false;
			switch(sequenceType)
			{
				case Type.Parallel: sequenceFinished = DoUpdateInParallel(); break;
				case Type.Queue: sequenceFinished = DoUpdateInQueue(); break;
				default: Debug.LogError($"[error][DoUpdate]. unknow SequenceType. {sequenceType}"); break;
			}

			if (!sequenceFinished) return false;

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

		public override void Sample (float factor, bool isFinished) { }
		protected override void OnUpdate(float factor, bool isFinished) { }

		public override void Play (bool forward)
		{
			bool haveAni = aniList?.Count > 0;
			if (haveAni)
			{
				for (int i = aniList.Count - 1; i >= 0; i--)
				{
					AniItem aniItem = aniList[i];
					if (aniItem.aniClip == null)
					{
						Debug.LogError($"[error][Play]. aniItem.aniClip == null. index: {i} will remove");
						aniList.RemoveAt(i);
						continue;
					}

					aniItem.aniClip.Play(forward);
				}
			}
			
			switch(sequenceType)
			{
				case Type.Parallel: PlayInParallel(forward); break;
				case Type.Queue: PlayInQueue(forward); break;
				default: Debug.LogError($"[error][Play]. unknow SequenceType. {sequenceType}"); return;
			}

			mAmountPerDelta = Mathf.Abs(amountPerDelta);
			if (!forward) mAmountPerDelta = -mAmountPerDelta;

			if (!enabled)
			{
				enabled = true;
				mStarted = false;
			}

			DoUpdate(true);
		}

		protected override void UpdateAniParam() 
		{
			IsAuto = false;
			duration = 0;
			animationCurve = null;
		}

		public override void ResetToBeginning ()
		{
			mStarted = false;
			mFactor = (amountPerDelta < 0f) ? 1f : 0f;

			if (aniList == null || aniList.Count == 0) return;

			aniList?.ForEach(aniItem => aniItem.aniClip?.ResetToBeginning());
		}


		private void ModiftListAni()
		{
			aniList.ForEach(item => 
			{
				item.aniClip.LoopType = DNUILoopType.None;
				item.aniClip.IsAuto = false;
				item.aniClip.SetAniLock(true);
			});
		}

		public void PlayInParallel (bool forward)
		{
			bool haveAni = aniList?.Count > 0;
			if (!haveAni)
			{
				waitTimeDic = null;
				aniParallel = null;
				return;
			}

			waitTimeDic = waitTimeDic ?? new Dictionary<AniItem, float>(aniList.Count);
			waitTimeDic.Clear();

			aniParallel = new List<AniItem>(aniList.Count);
			foreach(AniItem aniItem in aniList)
			{
				if (!aniItem.aniClip.gameObject.activeSelf) continue;

				aniParallel.Add(aniItem);
				waitTimeDic.Add(aniItem, IsAniClipFinished(aniItem) ? 0 : aniItem.delay);
			}
		}

		public void PlayInQueue (bool forward)
		{
			bool haveAni = aniList?.Count > 0;
			if (!haveAni)
			{
				aniQueue = null;
				return;
			}

			int start = forward ? 0 : aniList.Count - 1;
			int end = forward ? aniList.Count - 1 : 0;
			int step = forward ? 1 : -1;
			aniQueue = new Queue<AniItem>(aniList.Count);
			for (int i = start; forward ? i <= end : i >= end; i += step)
			{
				AniItem aniItem = aniList[i];
				if (!aniItem.aniClip.gameObject.activeSelf) continue;

				aniQueue.Enqueue(aniItem);
			}

			if (!AniUtility.SafePeek(aniQueue, out AniItem item)) return;

			waitTime = IsAniClipFinished(item) ? 0 : item.delay;
		}

		private bool DoUpdateInParallel()
		{
			if (aniParallel == null || aniParallel.Count == 0) return false;

			float delta = ignoreTimeScale && !useFixedUpdate ? Time.unscaledDeltaTime : Time.deltaTime;
			bool allFinished = true;
			for (int i = aniParallel.Count - 1; i >= 0; i--)
			{
				AniItem aniItem = aniParallel[i];
				if (waitTimeDic.TryGetValue(aniItem, out float leftTime) && leftTime > 0) 
				{
					waitTimeDic[aniItem] = leftTime - delta;
					allFinished = false;
					continue;
				}

				bool isAniFinished = aniItem.aniClip.DoUpdate(true);
				if (isAniFinished)
				{
					aniParallel.RemoveAt(i);
					waitTimeDic.Remove(aniItem);
				}

				allFinished &= isAniFinished;
			}

			return allFinished;
		}

		private bool DoUpdateInQueue()
		{
			if (!AniUtility.SafePeek(aniQueue, out AniItem aniItem)) return true;

			float delta = ignoreTimeScale && !useFixedUpdate ? Time.unscaledDeltaTime : Time.deltaTime;
			if (!SubtractWait(delta)) return false;

			bool isAniFinished = aniItem.aniClip.DoUpdate(true);
			if (!isAniFinished) return false;

			aniQueue.Dequeue();
			if (AniUtility.SafePeek(aniQueue, out AniItem nextItem)) 
			{
				waitTime = IsAniClipFinished(nextItem) ? 0 : nextItem.delay;
				return false;
			}
			
			return true;
		}

		private bool IsAniClipFinished(AniItem item)
		{
			UITweener aniClip = item.aniClip;

			// Debug.LogError($"[IsAniClipFinished]. tweenFactor: {aniClip.tweenFactor}");

			if (aniClip is DNUIAniSequence)
			{
				DNUIAniSequence aniSequence = (DNUIAniSequence)aniClip;
				bool isAllFinished = true;
				foreach(AniItem aniItem in aniSequence.aniList)
				{
					isAllFinished &= IsAniClipFinished(aniItem);
				}
				
				return isAllFinished;
			}
			else 
			{
				if (aniClip.direction == AnimationOrTween.Direction.Forward && aniClip.tweenFactor >= 1) return true;
				else if (aniClip.direction == AnimationOrTween.Direction.Reverse && aniClip.tweenFactor <= 0) return true;
			}

			return false;
		}

		private bool SubtractWait(float delt)
		{
			waitTime -= delt;

			return waitTime <= 0;
		}

		public void SetSequenceType(Type type)
		{
			Type preType = sequenceType;
			if (preType == type) return;

			sequenceType = type;

			waitTimeDic?.Clear();
			aniParallel?.Clear();
			aniQueue?.Clear();

			waitTimeDic = null;
			aniParallel = null;
			aniQueue = null;
			waitTime = 0;

			aniList?.ForEach(aniItem => aniItem.aniClip?.ResetToBeginning());
		}
	}
}