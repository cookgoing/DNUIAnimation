namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(IDNUIAnimation), true)]
	public class IDNUIAnimationEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			IDNUIAnimation aniCom = target as IDNUIAnimation;

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
			GUILayout.Label("循环类型", GUILayout.MaxWidth(100));
			DNUILoopType loopType = (DNUILoopType)EditorGUILayout.EnumPopup(aniCom.LoopType, GUILayout.MaxWidth(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("动画时长", GUILayout.MaxWidth(100));
			GUILayout.Label(aniCom.AniLen.ToString(), GUILayout.MaxWidth(100));
			GUILayout.Label("秒");
			GUILayout.EndHorizontal();

			if (GUI.changed)
			{
				aniCom.IsAuto = isAuto;
				aniCom.Delay = delay;
				aniCom.LoopType = loopType;
			}
		}
	}
}
