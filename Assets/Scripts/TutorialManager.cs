using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private Text TutText;

    public event Action OnTutBegin;
    public event Action OnTutEnd;
    public static TutorialManager instance;
    ControlTut currentTut;
    private void Awake()
    {
        instance = this;
        TutText = GetComponentInChildren<Text>();
        TutText.rectTransform.DOScale(0, 0);
    }

    public void StartTut(ControlTut CT, string tText)
    {
        OnTutBegin();
        switch (CT)
        {
            case ControlTut.Jump:
                InputManager.instance.OnTap += RewindTime;
                break;
            case ControlTut.Slide:
                InputManager.instance.OnSwipe += RewindTime;
                break;
            case ControlTut.Hold:
                InputManager.instance.OnTouchBeign += RewindTime;
                break;
            default:
                break;
        }
        currentTut = CT;
        SlowTime();
        TutText.text = tText;
        //Extensions.Debug(CT.ToString());
    }
    void RewindTime()
    {
        DOVirtual.Float(.15f, 1f, .5f, TimeScale).SetUpdate(true).OnComplete(() => InputReset());
        TutText.rectTransform.DOScale(0, .45f).SetUpdate(true).SetEase(Ease.InBounce);
    }
    void RewindTime(SwipeData sd)
    {
        if (sd.Direction != SwipeDirection.Down)
            return;
        DOVirtual.Float(.15f, 1f, .5f, TimeScale).SetUpdate(true).OnComplete(() => InputReset());
        TutText.rectTransform.DOScale(0, .45f).SetUpdate(true).SetEase(Ease.InBounce);
    }
    void InputReset()
    {
        switch (currentTut)
        {
            case ControlTut.Jump:
                InputManager.instance.OnTap -= RewindTime;
                break;
            case ControlTut.Slide:
                InputManager.instance.OnSwipe -= RewindTime;
                break;
            case ControlTut.Hold:
                InputManager.instance.OnTouchBeign -= RewindTime;
                break;
            default:
                break;
        }
        OnTutEnd();
    }
    void SlowTime()
    {
        DOVirtual.Float(1, .15f, .5f, TimeScale).SetUpdate(true);
        TutText.rectTransform.DOScale(1, .45f).SetUpdate(true).SetEase(Ease.OutBounce);
    }
    void TimeScale(float x)
    {
        Time.timeScale = x;
    }
}
public enum ControlTut
{
    Jump, Slide, Hold
}
