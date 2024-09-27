
namespace DNAni.Editor
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(DNUIAniSequence))]
	public class DNUIAniSequenceEditor : Editor
	{
		private Dictionary<DNUIAniSequence.Type, string> SequenceTypeChineseDic = new Dictionary<DNUIAniSequence.Type, string>()
		{
			{DNUIAniSequence.Type.Parallel, "同时播放"},
			{DNUIAniSequence.Type.Queue, "顺序播放"},
		};

		private string[] _chineseArr;
		private string[] chineseArr
		{
			get
			{
				if (_chineseArr == null) _chineseArr = SequenceTypeChineseDic.Values.ToArray();

				return _chineseArr;
			}
		}

		public override void OnInspectorGUI ()
		{
			DNUIAniSequence aniSequence = target as DNUIAniSequence;

			string selectedChinese = SequenceTypeChineseDic[aniSequence.SequenceType];
			int selectedIndex = Array.IndexOf(chineseArr, selectedChinese);

			GUI.changed = false;

			GUILayout.Space(5);
			GUILayout.Label(@"这个组件可以控制 UI Tweener 类型的动画组件的播放顺序
1.注意：添加到这个序列的动画，不能自动播放，也不能循环播放了。
2.注意：程序运行的时候，不要往里面添加动画组件，否则播放可能会不准确。", EditorStyles.wordWrappedLabel);
			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.Label("是否自动播放", GUILayout.MaxWidth(100));
			bool isAuto = EditorGUILayout.Toggle(aniSequence.IsAuto, GUILayout.MaxWidth(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("播放方式", GUILayout.MaxWidth(100));
			int newIndex = EditorGUILayout.Popup(selectedIndex, chineseArr, GUILayout.MaxWidth(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("动画时长", GUILayout.MaxWidth(100));
			GUILayout.Label(aniSequence.AniLen.ToString(), GUILayout.MaxWidth(50));
			GUILayout.Label("秒");
			GUILayout.EndHorizontal();

			GUILayout.Label("序列", GUILayout.MaxWidth(100));
			for (int i = 0; i < aniSequence.AniList.Count; )
			{
				DNUIAniSequence.AniItem aniItem = aniSequence.AniList[i];

				GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.Label($"[{i}]: ", GUILayout.MaxWidth(20));
				float newDelay = EditorGUILayout.FloatField(aniItem.delay, GUILayout.MaxWidth(50));
				UITweener newAniClip = (UITweener)EditorGUILayout.ObjectField(aniItem.aniClip, typeof(UITweener), true);
				bool isRemove = GUILayout.Button("-", GUILayout.MaxWidth(20));
				GUILayout.EndHorizontal();

				if (isRemove)
				{
					aniSequence.AniList.RemoveAt(i);
					continue;
				}

				if (newDelay != aniItem.delay || newAniClip != aniItem.aniClip)
					aniSequence.AniList[i] = new DNUIAniSequence.AniItem(newDelay, newAniClip);
				
				++i;
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			bool isAddAni = GUILayout.Button("+");
			GUILayout.EndHorizontal();

			if (isAddAni)
			{
				aniSequence.AniList.Add(new DNUIAniSequence.AniItem());
			}

			if (GUI.changed)
			{
				aniSequence.IsAuto = isAuto;
				if (newIndex != selectedIndex)
				{
					aniSequence.SetSequenceType(SequenceTypeChineseDic.First(en => en.Value == chineseArr[newIndex]).Key);
				}
			}
		}
	}
}
