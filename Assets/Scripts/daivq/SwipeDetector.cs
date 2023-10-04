using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.DesignPattern;

public class SwipeDetector : MonoBehaviour
{
	public BoolModifierWithRegisteredSource IsBlockPlayerInput { get; private set; } = new BoolModifierWithRegisteredSource();

	private Vector2 fingerDownPos;
	private Vector2 fingerUpPos;

	public bool detectSwipeAfterRelease = false;
	public Vector2Int SwipeDirection { get; private set; }

	public float SWIPE_THRESHOLD = 20f;

    // Update is called once per frame
    void Update ()
	{
		SwipeDirection = Vector2Int.zero;

		if (IsBlockPlayerInput.Value) return;

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				fingerUpPos = touch.position;
				fingerDownPos = touch.position;
			}

			//Detects Swipe while finger is still moving on screen
			if (touch.phase == TouchPhase.Moved) {
				if (!detectSwipeAfterRelease) {
					fingerDownPos = touch.position;
					DetectSwipe ();
				}
			}

			//Detects swipe after finger is released from screen
			if (touch.phase == TouchPhase.Ended) {
				fingerDownPos = touch.position;
				DetectSwipe ();
			}
		}
	}

	void DetectSwipe ()
	{
		
		if (VerticalMoveValue () > SWIPE_THRESHOLD && VerticalMoveValue () > HorizontalMoveValue ()) {
			Debug.Log ("Vertical Swipe Detected!");
			if (fingerDownPos.y - fingerUpPos.y > 0) {
				OnSwipeUp ();
			} else if (fingerDownPos.y - fingerUpPos.y < 0) {
				OnSwipeDown ();
			}
			fingerUpPos = fingerDownPos;

		} else if (HorizontalMoveValue () > SWIPE_THRESHOLD && HorizontalMoveValue () > VerticalMoveValue ()) {
			Debug.Log ("Horizontal Swipe Detected!");
			if (fingerDownPos.x - fingerUpPos.x > 0) {
				OnSwipeRight ();
			} else if (fingerDownPos.x - fingerUpPos.x < 0) {
				OnSwipeLeft ();
			}
			fingerUpPos = fingerDownPos;

		} else {
			Debug.Log ("No Swipe Detected!");
		}
	}

	float VerticalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.y - fingerUpPos.y);
	}

	float HorizontalMoveValue ()
	{
		return Mathf.Abs (fingerDownPos.x - fingerUpPos.x);
	}

	void OnSwipeUp ()
	{
		SwipeDirection = Vector2Int.up;
	}

	void OnSwipeDown ()
	{
		SwipeDirection = Vector2Int.down;
	}

	void OnSwipeLeft ()
	{
		SwipeDirection = Vector2Int.left;
	}

	void OnSwipeRight ()
	{
		SwipeDirection = Vector2Int.right;
	}
}
