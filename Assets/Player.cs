using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed = 5f;

	bool isMoving = false;
	bool isMovingWithMouse = false;
	Vector2 lastTouchPosition;
	Vector3 positionMin, positionMax, position;

	// Use this for initialization
	void Start () {
		positionMin = Camera.main.ScreenToWorldPoint (Vector3.zero);
		positionMax = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, Camera.main.pixelHeight, 0));
	}

	// Update is called once per frame
	void Update () {
		Vector2 currentTouchPosition = lastTouchPosition;

		if (Input.GetMouseButtonDown (0)) {
			isMoving = isMovingWithMouse = true;
			lastTouchPosition = currentTouchPosition = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp (0)) {
			isMoving = isMovingWithMouse = false;
		}

		if (isMovingWithMouse) {
			currentTouchPosition = Input.mousePosition;
		}

		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);

			switch (touch.phase) {
			case TouchPhase.Began:
				lastTouchPosition = currentTouchPosition = touch.position;
				break;
			case TouchPhase.Moved:
				isMoving = true;
				currentTouchPosition = touch.position;
				break;
			case TouchPhase.Ended:
				isMoving = false;
				break;
			case TouchPhase.Canceled:
				isMoving = false;
				break;
			}
		}

		if (isMoving) {
			Vector3 moveDirection = currentTouchPosition - lastTouchPosition;
			position = transform.position;
			position += speed * Time.deltaTime * moveDirection;
			position.x = Mathf.Clamp (position.x, positionMin.x, positionMax.x);
			position.y = Mathf.Clamp (position.y, positionMin.y, positionMax.y);
			transform.position = position;
			lastTouchPosition = currentTouchPosition;
		}
	}

	void OnDisable() {
		isMoving = isMovingWithMouse = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.name.StartsWith("Haze")) {
			Main.instance.GameOver ();
		}
	}
}
