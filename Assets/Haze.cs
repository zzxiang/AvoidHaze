using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haze : MonoBehaviour {

	public float alphaSpeed = 0.1f;
	public float scaleSpeed = 0.1f;

	SpriteRenderer spriteRenderer;
	Vector3 startScale;

	// Use this for initialization
	void OnEnable () {
		if (!spriteRenderer) {
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		Color color = spriteRenderer.color;
		color.a = Random.value;
		spriteRenderer.color = color;
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		float alphaDelta = Time.deltaTime * alphaSpeed;
		float scaleDelta = Time.deltaTime * scaleSpeed;
		Color color = spriteRenderer.color;
		color.a -= alphaDelta;
		if (color.a <= 0f) {
			Main.instance.RemoveHaze (this);
		} else {
			spriteRenderer.color = color;
			Vector3 scale = transform.localScale;
			scale.x += scaleDelta;
			scale.y += scaleDelta;
			transform.localScale = scale;
		}
	}

	public void Reset() {
		transform.localScale = startScale;
		Color color = spriteRenderer.color;
		color.a = 1f;
		spriteRenderer.color = color;
	}
}
