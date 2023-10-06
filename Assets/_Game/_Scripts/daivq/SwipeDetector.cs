using _Game.DesignPattern;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public bool detectSwipeAfterRelease;

    public float SWIPE_THRESHOLD = 20f;

    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    public BoolModifierWithRegisteredSource IsBlockPlayerInput { get; } = new();
    public Vector2Int SwipeDirection { get; private set; }

    // Update is called once per frame
    private void Update()
    {
        SwipeDirection = Vector2Int.zero;

        if (IsBlockPlayerInput.Value) return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            //Detects Swipe while finger is still moving on screen
            if (touch.phase == TouchPhase.Moved)
                if (!detectSwipeAfterRelease)
                {
                    fingerDownPos = touch.position;
                    DetectSwipe();
                }

            //Detects swipe after finger is released from screen
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }
        }
    }

    private void DetectSwipe()
    {

        if (VerticalMoveValue() > SWIPE_THRESHOLD && VerticalMoveValue() > HorizontalMoveValue())
        {
            Debug.Log("Vertical Swipe Detected!");
            if (fingerDownPos.y - fingerUpPos.y > 0)
                OnSwipeUp();
            else if (fingerDownPos.y - fingerUpPos.y < 0) OnSwipeDown();
            fingerUpPos = fingerDownPos;

        }
        else if (HorizontalMoveValue() > SWIPE_THRESHOLD && HorizontalMoveValue() > VerticalMoveValue())
        {
            Debug.Log("Horizontal Swipe Detected!");
            if (fingerDownPos.x - fingerUpPos.x > 0)
                OnSwipeRight();
            else if (fingerDownPos.x - fingerUpPos.x < 0) OnSwipeLeft();
            fingerUpPos = fingerDownPos;

        }
        else
        {
            Debug.Log("No Swipe Detected!");
        }
    }

    private float VerticalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    private float HorizontalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    private void OnSwipeUp()
    {
        SwipeDirection = Vector2Int.up;
    }

    private void OnSwipeDown()
    {
        SwipeDirection = Vector2Int.down;
    }

    private void OnSwipeLeft()
    {
        SwipeDirection = Vector2Int.left;
    }

    private void OnSwipeRight()
    {
        SwipeDirection = Vector2Int.right;
    }
}
