namespace DNAni
{
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("DNAni/Lean Come In")]
	[RequireComponent(typeof(UIPanel))]
	public partial class Magic_View_LeanComeIn : UITweener
	{
		public Camera Camera2D;
		private UIPanel uiPanel;
		private Dictionary<UIWidget, Vector3> uiWidgetOriLocalPosDic;

	#region local position
		public Vector3 from_localPos = Vector3.zero;
		public Vector3 to_localPos = Vector3.zero;
		public Vector3 value_localPos { get => transform.localPosition;  set => transform.localPosition = value; } 
		public float duration_localPos = 1f;
		public AnimationCurve aniCurve_localPos = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

		private float factor_localPos = 0f;
		private float amountPerDelta_localPos
		{
			get
			{
				if (duration_localPos == 0f) return 1000f;

				return Mathf.Abs(1f / duration_localPos) * Mathf.Sign(mAmountPerDelta);
			}
		}
	#endregion

	#region local rotation
		public Quaternion from_rotation = Quaternion.identity;
		public Quaternion to_rotation = Quaternion.identity;

		public Quaternion value_rotation { get => transform.rotation;  set => transform.rotation = value; }
	#endregion

		protected override void Awake()
		{
			base.Awake();

			uiPanel = GetComponent<UIPanel>();
			uiWidgetOriLocalPosDic = new Dictionary<UIWidget, Vector3>();
			uiPanel.onAddWidget += OnPanelAddWidget;
			uiPanel.onRemoveWidget += OnPanelRemnoveWidget;
			uiPanel.generateUV2 = true;
		}

		private void OnDestroy()
		{
			uiPanel.onAddWidget -= OnPanelAddWidget;
			uiPanel.onRemoveWidget -= OnPanelRemnoveWidget;
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

			factor_localPos += (duration_localPos == 0f) ? 1f : amountPerDelta_localPos * delta;
			mFactor += (duration == 0f) ? 1f : amountPerDelta * delta;

			if (style == Style.Loop)
			{
				if (factor_localPos > 1f)
				{
					factor_localPos -= Mathf.Floor(factor_localPos);
				}

				if (mFactor > 1f)
				{
					mFactor -= Mathf.Floor(mFactor);
				}
			}
			else if (style == Style.PingPong)
			{
				if (mFactor > 1f)
				{
					factor_localPos = duration / duration_localPos - (factor_localPos - Mathf.Floor(factor_localPos));

					mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
					mAmountPerDelta = -mAmountPerDelta;
				}
				else if (mFactor < 0f)
				{
					factor_localPos = -factor_localPos;
					factor_localPos -= Mathf.Floor(factor_localPos);

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

			factor_localPos = UpdateFactor(factor_localPos);
			factor = UpdateFactor(factor);

			float rotationFactor = (animationCurve != null) ? animationCurve.Evaluate(factor) : factor;

			OnUpdate(rotationFactor, isFinished);
		}

		protected override void OnUpdate(float factor, bool isFinished) 
		{
			float localPosFactor = (aniCurve_localPos != null) ? aniCurve_localPos.Evaluate(factor_localPos) : factor_localPos;

			value_localPos = Vector3.Lerp(from_localPos, to_localPos, localPosFactor);
			value_rotation = Quaternion.Lerp(from_rotation, to_rotation, factor);

			MarkAllWidgtesChanged();
		}

		public override void ResetToBeginning ()
		{
			mStarted = false;

			bool isBackward = amountPerDelta < 0f;
			mFactor = isBackward ? 1f : 0f;
			factor_localPos = isBackward ? duration / duration_localPos : 0f;

			Sample(mFactor, false);
		}


		public void MarkAllWidgtesChanged()
		{
			foreach(UIWidget widget in uiPanel.widgets)
			{
				widget.MarkAsChanged();
			}
		}

		private void OnPanelAddWidget(UIWidget widget)
		{
			widget.onPostFill += OnPannelWidgetPostFill;
			uiWidgetOriLocalPosDic[widget] = widget.transform.localPosition;
		}

		private void OnPanelRemnoveWidget(UIWidget widget) 
		{
			widget.onPostFill -= OnPannelWidgetPostFill;
			uiWidgetOriLocalPosDic.Remove(widget);
		} 

		private void OnPannelWidgetPostFill(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
		{
			Vector3 oldLocalPos = uiWidgetOriLocalPosDic[widget];
			Vector3 oldWorldPos = uiPanel.transform.TransformPoint(oldLocalPos);
			Vector2 newlocalPos = AniUtility.Point2Dto3D(Camera2D, oldLocalPos, oldWorldPos.z);

			widget.transform.localPosition = new Vector3(newlocalPos.x, newlocalPos.y, oldLocalPos.z);

			for (int i = 0; i < verts.Count; ++i)
			{
				Vector3 oldlocalVert = verts[i];
				Vector3 oldWorldVert = widget.transform.TransformPoint(oldlocalVert);
				Vector2 newlocalVert = AniUtility.Point2Dto3D(Camera2D, oldlocalVert, oldWorldVert.z);

				verts[i] = new Vector3(newlocalVert.x, newlocalVert.y, oldlocalVert.z);
			}
		}


		protected override void UpdateAniParam() 
		{
			IsAuto = false;

			from_localPos = transform.localPosition + Vector3.up * 500 + Vector3.forward * 1000;
			to_localPos = transform.localPosition;
			duration_localPos = 0.3f;
			aniCurve_localPos = new AnimationCurve();
			aniCurve_localPos.AddKey(new Keyframe(0, 0, 2, 2));
			aniCurve_localPos.AddKey(new Keyframe(1, 1, 0, 0));

			from_rotation = Quaternion.FromToRotation(transform.up, Vector3.forward);//(0.7, 0, 0, 0.7)
			to_rotation = transform.localRotation;//(0, 0, 0, 1)
			duration = 0.3f;
			animationCurve = new AnimationCurve();
			animationCurve.AddKey(new Keyframe(0, 0, 2, 2));
			animationCurve.AddKey(new Keyframe(1, 1, 0, 0));

		}

		[ContextMenu("PrintCurQuaternion")]
		private void PrintCurQuaternion()
		{
			print(transform.rotation);
		}
	}
}