using UnityEngine;
using System.Collections;

public class SpaceShipMove : MonoBehaviour {
	Rigidbody rigid;

	private float speed = 0.8F;
	private float turnSpeed = 0.15F;
	// Use this for initialization
	void Start () {
		rigid = transform.GetComponent<Rigidbody> ();
		rigid.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void FixedUpdate() {
		if(Input.GetButton ("Jump")) {
			rigid.AddRelativeForce (Vector3.forward * speed); 
		}

		rigid.AddRelativeTorque((Input.GetAxis ("Vertical")) * turnSpeed, 0, 0);
		rigid.AddRelativeTorque(0, (Input.GetAxis ("Horizontal")) * turnSpeed, 0); 


	}
}