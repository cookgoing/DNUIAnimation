namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Base_Grid_DropInAni))]
	public class Base_Grid_DropInAniEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			IDNUIAnimation aniCom = target as IDNUIAnimation;

			GUILayout.Space(5);
			GUILayout.Label(@"每个子物体都随机延迟掉入出现
这个动画最好用在UIGrid上", EditorStyles.wordWrappedLabel);
			GUILayout.Space(5);
			
			GUI.changed = false;

			GUILayout.BeginHorizontal();
			GUILayout.Label("是否自动播放", GUILayout.MaxWidth(100));
			bool isAuto = EditorGUILayout.Toggle(aniCom.IsAuto, GUILayout.MaxWidth(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("播放延迟", GUILayout.MaxWidth(100));
			float delay = EditorGUILayout.FloatField(aniCom.Delay, GUILayout.MaxWidth(50));
			GUILayout.Label("秒");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("动画时长", GUILayout.MaxWidth(100));
			GUILayout.Label(aniCom.AniLen.ToString(), GUILayout.MaxWidth(50));
			GUILayout.Label("秒");
			GUILayout.EndHorizontal();

			if (GUI.changed)
			{
				aniCom.IsAuto = isAuto;
				aniCom.Delay = delay;
			}
		}
	}
}