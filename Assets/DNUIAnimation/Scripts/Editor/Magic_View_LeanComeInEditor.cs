
namespace DNAni.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Magic_View_LeanComeIn))]
	public class Magic_View_LeanComeInEditor : IDNUIAnimationEditor
	{

		public override void OnInspectorGUI ()
		{
			GUILayout.Space(5);
			GUILayout.Label(@"倾斜着，快速移动进来的动画。
这个动画最好用在界面上", EditorStyles.wordWrappedLabel);
			GUILayout.Space(5);
			base.OnInspectorGUI ();
		}
	}
}
