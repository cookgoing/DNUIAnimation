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

}
