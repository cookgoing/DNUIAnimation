using System.Collections.Generic;
using DNAni;
using UnityEditor;
using UnityEngine;

public class TestAni : MonoBehaviour
{
	public UIButton playForwardBtn;
	public UIButton playBackwardBtn;
	public UIButton pauseBtn;
	public UITweener dNUIAni;

    void Start()
    {
		EventDelegate.Set(playForwardBtn.onClick, PlayForward);
		EventDelegate.Set(playBackwardBtn.onClick, PlayBackward);
		EventDelegate.Set(pauseBtn.onClick, Pause);

		dNUIAni.OnAniComplete += TestAniComplete;

		dNUIAni.ResetToBeginning();
    }

	void PlayForward()
	{
		dNUIAni.PlayForward();
		// dNUIAni.ResetToBeginning();
	}

	void PlayBackward()
	{
		dNUIAni.PlayBackward();
		// dNUIAni.ResetToBeginning();
	}

	void Pause()
	{
		dNUIAni.Pause();
	}

	void TestAniComplete(bool isForward)
	{
		print($"[TestAniComplete]. isForward: {isForward}");
	}


	[ContextMenu("TestNewFuc")]
	void TestNewFuc()
	{
		TestNewFuc2 f = new TestNewFuc2();
		f.Test();
	}

	[ContextMenu("TestQueue")]
	void TestQueue()
	{
		Queue<int> q = new Queue<int>();
		q.Enqueue(1);
		q.Enqueue(2);

		for (int i = 0; i < 100; ++i)
		{
			if (!AniUtility.SafeDequeue(q, out int value)) break;

			print($"[test]. i: {i}; value: {value}");
		}
	}
}

public class TestNewFuc
{

	public void Test()
	{
		Call();
	}

	protected virtual void Call()
	{
		Debug.Log("Call");
	}
}

public class TestNewFuc2 : TestNewFuc
{
	protected override void Call()
	{
		Debug.Log("Call 2");
	}
}
