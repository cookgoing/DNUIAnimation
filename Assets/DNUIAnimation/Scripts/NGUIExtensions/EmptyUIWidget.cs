using System.Collections.Generic;
using UnityEngine;

namespace DNAni.UIExtension
{
	public class EmptyUIWidget : UIWidget
	{
		public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols) { }
	}
}
