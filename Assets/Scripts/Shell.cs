using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

	public Rigidbody myRigidbody;
	public float forceMin;
	public float forceMax;

	float lifetime = 4;
	float fadetime = 2;

	// Use this for initialization
	void Start () {
		// Slighty randomize the force added to the shell, to make it more realistic.
		float force =  Random.Range(forceMin, forceMax);
		// we want to have the shell popping out from the right side of the gun.
		myRigidbody.AddForce(transform.right * force);
		// Slighty randomize the rotation of the shells as well.
		myRigidbody.AddTorque(Random.insideUnitSphere * force);

		StartCoroutine(Fade());
	}
	
	IEnumerator Fade() {
		yield return new WaitForSeconds(lifetime);

		float percent = 0;
		float fadeSpeed = 1 / fadetime;
		Material mat = GetComponent<Renderer>().material;
		Color initialColor = mat.color;

		while (percent < 1) {
			percent +=  Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp(initialColor, Color.clear, percent);
			yield return null;
		}

		// After "percent" reaches 1 and therefore completely invisible, we destroy the shell.
		Destroy (gameObject);
	}
}
