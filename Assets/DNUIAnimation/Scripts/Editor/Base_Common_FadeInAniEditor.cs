namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Common_FadeInAni))]
	public class Base_Common_FadeInAniEditor : IDNUIAnimationEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label("UI 淡入动画");
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}
