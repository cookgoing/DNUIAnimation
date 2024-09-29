namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Common_DropInAni))]
	public class Base_Common_DropInAniEditor : IDNUIAnimationEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label("掉进来的动画");
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}