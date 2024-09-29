namespace DNAni
{
	using UnityEngine;

	[AddComponentMenu("DNAni/Drop In")]
	public partial class Base_Common_DropInAni : Base_Common_FadeInAni
	{
		public Vector3 from_pos;
		public Vector3 to_pos;
		UIWidget widget_pos;

		public Vector3 value_pos
		{
			get
			{
				return transform.localPosition;
			}
			set
			{
				if (widget_pos == null || !widget_pos.isAnchored)
				{
					transform.localPosition = value;
				}
				else
				{
					value -= transform.localPosition;
					NGUIMath.MoveRect(widget_pos, value.x, value.y);
				}
			}
		}


		protected override void Awake()
		{
			widget_pos = GetComponent<UIWidget>();
			base.Awake();
		}

		protected override void OnUpdate (float factor, bool isFinished) 
		{
			base.OnUpdate(factor, isFinished);

			value_pos = from_pos * (1f - factor) + to_pos * factor; 
		}

		protected override void UpdateAniParam() 
		{
			base.UpdateAniParam();

			float offset = Screen.height * 0.03f; //widget_pos?.height ?? Screen.height * 0.03f;

			from_pos = transform.localPosition + Vector3.up * offset;
			to_pos = transform.localPosition;
		}
	}
}
