
namespace DNAni
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Unity.Mathematics;
	using UIExtension;

	[AddComponentMenu("DNAni/Grid_DropIn")]
	[RequireComponent(typeof(UIGrid))]
	public partial class Base_Grid_DropInAni : UITweener
	{
		private class ItemAni
		{
			public Transform tran;
			public UIWidget widget;
			public Vector3 from_localPos;
			public Vector3 to_localPos;
			public float from_fade;
			public float to_fade;
			public float dely;

			public float factor;
			public float timer;
			public Vector3 Value_localPos
			{
				get => tran?.localPosition ?? Vector3.zero;
				set
				{
					if (tran == null)
					{
						Debug.LogError("[error][value_localPos]. tran == null");
						return;
					}

					if (widget == null || !widget.isAnchored)
					{
						tran.localPosition = value;
					}
					else
					{
						value -= tran.localPosition;
						NGUIMath.MoveRect(widget, value.x, value.y);
					}
				}
			}
			public float Value_fade
			{
				get => widget?.alpha ?? 0;
				set 
				{
					if (widget == null)
					{
						Debug.LogError("[error][value_fade]. widget == null");
						return;
					}

					widget.alpha = value;
				} 
			}
		}

		private UIGrid uiGrid;
		private List<ItemAni> itemAniList;
		private HashSet<EmptyUIWidget> newWidgetList;

		public override float AniLen
		{
			get
			{
				if (itemAniList == null || itemAniList.Count == 0) return duration * 2;

				float maxDelyInClip = 0;
				foreach(ItemAni itemAni in itemAniList) maxDelyInClip = math.max(maxDelyInClip, itemAni.dely);

				return duration + maxDelyInClip;
			}
		}

		protected override void Awake()
		{
			base.Awake();

			uiGrid = GetComponent<UIGrid>();
		}

		protected override void Start ()
		{
			base.Start();

			UpdateChildrenToAni();
		}

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
			if (itemAniList == null || itemAniList.Count == 0) return true;

			float factorDelt = (duration == 0f) ? 1f : amountPerDelta * delta;
			bool isAllFinished = true;
			foreach(ItemAni itemAni in itemAniList)
			{
				itemAni.timer += delta;
				if (itemAni.timer < itemAni.dely) 
				{
					isAllFinished = false;
					continue;
				}

				itemAni.factor = Mathf.Clamp01(itemAni.factor + factorDelt);
				bool isFinished = factorDelt > 0 ? itemAni.factor >= 1 : itemAni.factor <= 0;

				isAllFinished &= isFinished;
			}

			if (isAllFinished) enabled = false;
			Sample(0, isAllFinished);

			if (isAllFinished && current != this)
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

			return isAllFinished;
		}

		public override void Sample (float factor, bool isFinished) => OnUpdate(factor, isFinished);

		protected override void OnUpdate(float factor, bool isFinished)
		{
			if (itemAniList == null || itemAniList.Count == 0) return;

			foreach(ItemAni itemAni in itemAniList)
			{
				float val = (animationCurve != null) ? animationCurve.Evaluate(itemAni.factor) : itemAni.factor;

				itemAni.Value_localPos = Vector3.Lerp(itemAni.from_localPos, itemAni.to_localPos, val);
				itemAni.Value_fade = Mathf.Lerp(itemAni.from_fade, itemAni.to_fade, val);
			}
		}

		public override void Play (bool forward)
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
			if (!forward) mAmountPerDelta = -mAmountPerDelta;

			if (!enabled)
			{
				enabled = true;
				mStarted = false;
			}

			if (itemAniList != null)
				foreach(ItemAni itemAni in itemAniList)
					itemAni.timer = 0;

			DoUpdate();
		}

		public override void ResetToBeginning() 
		{
			if (itemAniList == null || itemAniList.Count == 0) return;

			foreach(ItemAni itemAni in itemAniList)
			{
				itemAni.factor = 0;
				itemAni.timer = 0;
			}

			mStarted = false;
			Sample(0, false);
		}

		protected override void UpdateAniParam() 
		{
			LoopType = DNUILoopType.None;
			duration = 0.3f;
			animationCurve = new AnimationCurve();
			animationCurve.AddKey(new Keyframe(0, 0, 2, 2));
			animationCurve.AddKey(new Keyframe(1, 1, 0, 0));
		}


		private ItemAni CreateItemAni(Transform itemTran)
		{
			UIWidget uiwidget = itemTran.gameObject.GetComponent<UIWidget>();			
			float offset = uiwidget?.height * 0.5f ?? Screen.height * 0.03f;
			float dely = UnityEngine.Random.Range(0f, 0.5f);
			if (newWidgetList == null) newWidgetList = new HashSet<EmptyUIWidget>();

			if (uiwidget == null)
			{
				// todo: size 修改一下
				uiwidget = itemTran.gameObject.AddComponent<EmptyUIWidget>();
				newWidgetList.Add(uiwidget as EmptyUIWidget);
			}

			ItemAni result = new ItemAni()
			{
				tran = itemTran,
				widget = uiwidget,
				from_localPos = itemTran.localPosition + Vector3.up * offset,
				to_localPos = itemTran.localPosition,
				from_fade = 0,
				to_fade = 1,
				dely = dely,
				factor = 0,
				timer = 0,
			};

			return result;
		}

		public void MakeComplete() 
		{
			if (itemAniList == null || itemAniList.Count == 0) return;
			
			foreach(ItemAni itemAni in itemAniList)
			{
				itemAni.factor = 1;
				itemAni.timer = itemAni.dely;
				itemAni.Value_localPos = itemAni.to_localPos;
				itemAni.Value_fade = itemAni.to_fade;
			}
		}

		public void UpdateChildrenToAni()
		{
			MakeComplete();

			if (newWidgetList != null && newWidgetList.Count > 0) 
				foreach(EmptyUIWidget emptyWidget in newWidgetList) 
					Destroy(emptyWidget);

			if (itemAniList == null) itemAniList = new List<ItemAni>();
			else itemAniList.Clear();

			if (newWidgetList == null) newWidgetList = new HashSet<EmptyUIWidget>();
			else newWidgetList.Clear();

			GC.Collect();

			uiGrid?.Reposition();
			foreach (Transform child in transform)  itemAniList.Add(CreateItemAni(child));
		}

		public void ClearItemAnis()
		{
			MakeComplete();

			if (newWidgetList != null && newWidgetList.Count > 0) 
				foreach(EmptyUIWidget emptyWidget in newWidgetList) 
					Destroy(emptyWidget);

			itemAniList = null;
			newWidgetList = null;
			GC.Collect();
		}

	}
}
