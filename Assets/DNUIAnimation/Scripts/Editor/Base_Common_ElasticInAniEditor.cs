namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Common_ElasticInAni))]
	public class Base_Common_ElasticInAniEditor : IDNUIAnimationEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label("弹出来的动画");
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}