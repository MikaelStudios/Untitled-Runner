using System;
using UnityEngine;


#if UNITY_EDITOR
using Input = InputWrapper.Input;
#endif

public class InputManager : MonoBehaviour
{
    [Header("TOUCH-HOLD PARAMETERS")]

    [SerializeField] private float holdTime = .8f;


    [Header("SWIPE PARAMETERS")]

    [SerializeField] private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField] private float minDistanceForSwipe = 20f;

    [Space]
    [SerializeField] private bool DebugStatements;

    private float acumTime = 0;
    private bool TouchBegin;
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private Vector2 beginPosition, endPosition;
    private Touch touch;

    public event Action OnTap = delegate { };
    public event Action OnHold = delegate { };
    public event Action OnHoldEnd = delegate { };
    public event Action<SwipeData> OnSwipe = delegate { };
    public event Action OnTouchBeign = delegate { };

    #region New Tap Properties
    Vector2 touchPos;
    int SCount = 0; // Count of started touches
    int MCount = 0; // Count of ended touches
    int ECount = 0; // Count of moved touches
    int LastPhaseHappend; // 1 = S, 2 = M, 3 = E
    float TouchTime = 0; // Time elapsed between touch beginning and ending
    float StartTouchTime = 0; // Time.realtimeSinceStartup at start of touch
    float EndTouchTime = 0; // Time.realtimeSinceStartup at end of touch
    #endregion

    public static InputManager instance;
    private void Awake()
    {

        instance = this;
        if (DebugStatements)
        {
            OnHold += () => Extensions.Debug("Hold");
            OnSwipe += DebugSwipe;
            OnTap += () => Extensions.Debug("Tap");
        }
    }

    private void Update()
    {
        touchPos = Vector3.zero;
        if (Input.touchCount > 0)
        {
            OnTouchBeign();
            StackOverflowTapMethod();
            SwipeInputCheck();
            CheckForHold();
        }
    }

    private void CheckForHold()
    {
        acumTime += Input.GetTouch(0).deltaTime;
        if (acumTime >= holdTime)
        {
            OnHold();
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            {
                acumTime = 0;
                OnHoldEnd();
            }
        }
        else
            TouchBegin = false;
    }

    private void StackOverflowTapMethod()
    {
        Touch currentTouch = Input.GetTouch(0);
        switch (currentTouch.phase)
        {
            case TouchPhase.Began:
                if (LastPhaseHappend != 1)
                {
                    SCount++;
                    StartTouchTime = Time.realtimeSinceStartup;
                }
                LastPhaseHappend = 1;
                break;

            case TouchPhase.Moved:
                if (LastPhaseHappend != 2)
                {
                    MCount++;
                }
                LastPhaseHappend = 2;
                break;

            case TouchPhase.Ended:
                if (LastPhaseHappend != 3)
                {
                    ECount++;
                    EndTouchTime = Time.realtimeSinceStartup;
                    TouchTime = EndTouchTime - StartTouchTime;
                }
                LastPhaseHappend = 3;
                break;
        }
        if (SCount == ECount && ECount != MCount && TouchTime < 1)
        // TouchTime for a tap can be further defined
        {
            OnTap();
            touchPos = currentTouch.position;
            MCount++;
        }
    }


    #region  DebugMethods

    void DebugSwipe(SwipeData data)
    {
        Extensions.Debug("Swiped in the direction " + data.Direction.ToString());
    }

    #endregion


    #region Swipe Methods
    private void SwipeInputCheck()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    private void SendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPosition = fingerDownPosition,
            EndPosition = fingerUpPosition
        };
        OnSwipe(swipeData);
    }
    #endregion

}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public SwipeDirection Direction;
}
public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}