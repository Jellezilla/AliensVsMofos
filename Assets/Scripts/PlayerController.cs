using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (GunController))]
public class PlayerController : MonoBehaviour {

	// Handling
	public float rotationSpeed = 450;
	public float walkSpeed = 2;
	public float runSpeed = 8;

	// System
	private Quaternion targetRotation;

	// Components
	//public Gun gun;
	GunController gunController;

	private CharacterController controller;
	private Camera cam;

	void Start () {
		gunController = GetComponent<GunController>();
		controller = GetComponent<CharacterController>();
		cam = Camera.main;
	}

	void Update () {
		ControlMouse();
		//ControlWASD();


		// Old Gun controlls
//		if (Input.GetButtonDown("Shoot")) {
//			gun.Shoot();
//		}
//		else if (Input.GetButton("Shoot")) {
//			gun.ShootContinuous();
//		}


		// Weapon Input
		if (Input.GetMouseButton(0) || Input.GetKeyDown("space")) {
			gunController.OnTriggerHold();
		}

		if (Input.GetMouseButtonUp(0)) {
			gunController.OnTriggerRelease();
		}

	}

	void ControlMouse() {

		Vector3 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y,cam.transform.position.y - transform.position.y));
		targetRotation = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x,0,transform.position.z));
		transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetRotation.eulerAngles.y,rotationSpeed * Time.deltaTime);

		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
		Vector3 motion = input;
		motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1)?.7f:1;
		//motion *= (Input.GetButton("Run"))?runSpeed:walkSpeed;
		motion += Vector3.up * -8;
		
		controller.Move(motion * Time.deltaTime);
	}

	/*void ControlWASD() {
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
		
		if (input != Vector3.zero) {
			targetRotation = Quaternion.LookRotation(input);
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetRotation.eulerAngles.y,rotationSpeed * Time.deltaTime);
		}
		
		Vector3 motion = input;
		motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1)?.7f:1;
		//motion *= (Input.GetButton("Run"))?runSpeed:walkSpeed;
		motion += Vector3.up * -8;
		
		controller.Move(motion * Time.deltaTime);
	}*/
	
}
