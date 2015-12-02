using UnityEngine;
using System.Collections;

public class ShellOld : MonoBehaviour {

	private float lifeTime = 5;
	private Material mat;
	private Color originalCol;
	private float fadePercent;
	private float deathTime;
	private bool fading;

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;
		originalCol = mat.color;
		deathTime = Time.time + lifeTime;

		StartCoroutine("Fade");
	}
	
	IEnumerator Fade() {
		while(true) {
			yield return new WaitForSeconds(.2f);

			if (fading) {
				// Fade the shells away.
				fadePercent += Time.deltaTime;
				mat.color = Color.Lerp(originalCol, Color.clear, fadePercent);

				if (fadePercent >1) {
					// Destroy the shells, when they're complete transparent (faded completely).
					Destroy(gameObject);
				}
			}
			else {
				if (Time.time > deathTime) {
					fading = true;
				}
			}
		}
	}

	void OnTriggerEnter (Collider c) {
		if (c.tag == "Ground") {
			// Set rigidbody of shells to sleep when they touch the ground, so the shells will stop wobbling around at eat the performance.
			GetComponent<Rigidbody>().Sleep();
		}
	}
}
