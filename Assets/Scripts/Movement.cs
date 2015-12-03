using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (GunController))]
public class Movement : MonoBehaviour {

    Rigidbody rigid;
    private float h, v;
    public float maxSpeed;

	public float rotationSpeed = 450;
	public float walkSpeed = 2;
	public float runSpeed = 8;

	Animator anim;

	private Quaternion targetRotation;
	GunController gunController;
	
	private CharacterController controller;

	public AudioClip footstepSound;
	//public AudioClip[] footstepSounds;



	// Use this for initialization
	void Start () {
        rigid = transform.GetComponent<Rigidbody>();
        //maxSpeed = 3.0F;
		gunController = GetComponent<GunController>();
		controller = GetComponent<CharacterController>();

		anim = transform.GetComponent<Animator>();

		//Loading the items into the array
//		footstepSounds =  new AudioClip[]{(AudioClip)Resources.Load("Audio/footsteps-gravel"),
//			(AudioClip)Resources.Load("footsteps-gravel2"), 
//			(AudioClip)Resources.Load("footsteps-gravel3"), 
//			(AudioClip)Resources.Load("footsteps-gravel4")};
	}
	
	// Update is called once per frame
	void Update () {
		ControlMouse();
		//ControlWASD();
		anim.SetFloat ("Forward", rigid.velocity.magnitude);
		//anim.SetFloat ("Turn", 
		
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
    void FixedUpdate()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        
        if (v > 0)
        {
            rigid.AddForce(GetIsoDir(Vector3.right), ForceMode.Impulse);
        } else if (v < 0)
        {
            rigid.AddForce(GetIsoDir(Vector3.right) * -1, ForceMode.Impulse);
        }

        if(h < 0)
        {
            rigid.AddForce(GetIsoDir(Vector3.forward), ForceMode.Impulse);
        } else if (h >   0)
        {
            rigid.AddForce(GetIsoDir(Vector3.forward) * -1, ForceMode.Impulse);
        }

        if (rigid.velocity.magnitude > maxSpeed)
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }
    }
    public Vector3 GetIsoDir(Vector3 dir) {
        dir = Quaternion.Euler(0, -45, 0) * dir;
        return dir;
    }

	void ControlMouse() {

		float speed = 30.0f;
		Plane playerPlane = new Plane(Vector3.up, transform.position);
		
		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		// Determine the point where the cursor ray intersects the plane.
		// This will be the point that the object must look towards to be looking at the mouse.
		// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
		//   then find the point along that ray that meets that distance.  This will be the point
		//   to look at.
		float hitdist = 0.0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast (ray, out hitdist)) 
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint = ray.GetPoint(hitdist);
			
			// Determine the target rotation.  This is the rotation if the transform looks at the target point.
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
			
			// Smoothly rotate towards the target point.
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
		}

	
	}

	public void PlayFootstepSound() {
		GetComponent<AudioSource>().PlayOneShot(footstepSound, 0.5f);

			//if (GetComponent<AudioSource>().isPlaying) return;
//			GetComponent<AudioSource>().clip = footstepSounds[Random.Range(0,footstepSounds.Length)];
//			GetComponent<AudioSource>().Play();
	}
}
