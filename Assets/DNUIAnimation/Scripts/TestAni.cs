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
    }

	void PlayForward()
	{
		dNUIAni.PlayForward();
	}

	void PlayBackward()
	{
		dNUIAni.PlayBackward();
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
