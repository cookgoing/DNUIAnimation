namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Common_ScaleInAni))]
	public class Base_Common_ScaleInAniEditor : IDNUIAnimationEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label("带缩放的淡入动画");
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}