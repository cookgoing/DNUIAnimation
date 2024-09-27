namespace DNAni.Editor
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(IDNUIAnimation), true)]
	public class IDNUIAnimationEditor : Editor
	{
		private Dictionary<DNUILoopType, string> chineseEnDic = new Dictionary<DNUILoopType, string>()
		{
			{DNUILoopType.None, "播放一次"},
			{DNUILoopType.Restart, "从头循环"},
			{DNUILoopType.Yoyo, "来回循环"},
		};

		private string[] _chineseEnValue;
		private string[] chineseEnValue
		{
			get
			{
				if (_chineseEnValue != null) return _chineseEnValue;

				_chineseEnValue = chineseEnDic.Values.ToArray();
				return _chineseEnValue;
			}
		}

		public override void OnInspectorGUI ()
		{
			IDNUIAnimation aniCom = target as IDNUIAnimation;

			string selectedChinese = chineseEnDic[aniCom.LoopType];
			int selectedIndex = Array.IndexOf(chineseEnValue, selectedChinese);

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
			int newIndex = EditorGUILayout.Popup(selectedIndex, chineseEnValue, GUILayout.MaxWidth(100));
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

				if (selectedIndex != newIndex)
				{
					aniCom.LoopType = chineseEnDic.First(chineseEn => chineseEn.Value == chineseEnValue[newIndex]).Key;
				}
			}
		}
	}
}
