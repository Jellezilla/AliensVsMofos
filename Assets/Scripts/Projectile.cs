using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Color trailColor;

	float speed = 10;
	float lifeTime = 3;


	void Start() {
		Destroy (gameObject, lifeTime);

		GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
	}


	public void SetSpeed(float newSpeed) {
		speed = newSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		// Make the projectile move forward.
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
}
