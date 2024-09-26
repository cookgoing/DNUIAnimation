namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Common_MoveInAni))]
	public class Base_Common_MoveInAniEditor : IDNUIAnimationEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label("带移动的淡入动画");
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}