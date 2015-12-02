using UnityEngine;
using System.Collections;

public class EnemyAttributes : MonoBehaviour {

	public int maxHealth = 100;
	public int currentHealth {get; private set;}
	public bool alive {get; private set;}

	public float speed = 2;
	Vector3[] path;
	int targetIndex;
	public bool destinationReached = false;

	float accuracy = 20;
	public int damage = 20;
	float criticalChange = 20;
	float criticalMultiplier = 1.5f;

	Animator anim;
	// Use this for initialization
	void Start () {
		alive = true;
		currentHealth = maxHealth;
		anim = GetComponent<Animator>();
	}


	public IEnumerator ShootAtTarget(Vector3 target, int numberOfShots){
		for (int i = 0; i < numberOfShots; i++){

			anim.SetTrigger("Shoot");
//			print ("enemy shooting");

			int hitAccuracy = Random.Range (1,100);
			bool hit = (hitAccuracy < accuracy ? true : false);         

			if (hit){
//				print ("enemy hitting");
				float finalDamage = damage;
				int rand = Random.Range (1,100);
				if (rand <= criticalChange){
					finalDamage *= criticalMultiplier;
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().ApplyDamage((int)finalDamage);
				}
			}
		
			float delay = Random.Range (0.3f, 0.8f);
			yield return new WaitForSeconds(delay);
		}

	}
	
	// Update is called once per frame
	void Update () {


		anim.SetBool("Alive", alive);
		if (currentHealth <= 0 && alive){
			alive = false;
			StopAllCoroutines();
		}

		if (Input.GetKeyDown(KeyCode.Space)){
			ApplyDamage(200);
		}

		if (Input.GetKeyDown(KeyCode.LeftCommand)){
			anim.SetTrigger("Shoot");
		}
	}

	public void ApplyDamage(int dmg){
		if (currentHealth > 0)
			currentHealth -= dmg;
	}
	
	public void MakeRequest(Vector3 target){
		PathRequestManager.RequestPath(transform.position, target, OnPathFound);
	}
	
	public void OnPathFound(Vector3[] newPath, bool pathSuccesful){
		
		if (pathSuccesful){
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
	
	public void StopPathFollowing(){
		StopCoroutine("FollowPath");
		
	}
	
	IEnumerator FollowPath(){
		if (path.Length > 0){
			Vector3 currentWaypoint = path[0];
			Vector3 lastWaypoint = path[path.Length-1];
			targetIndex = 0;
			while (true){
				if (transform.position == currentWaypoint){
					targetIndex++;
					if (targetIndex >= path.Length){
						yield break;
					}
					currentWaypoint = path[targetIndex];
				}
				if (alive)
					transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
				
				if (transform.position == lastWaypoint ){
					destinationReached = true;
				}
				yield return null; 
			}
			yield return null;
		}
		
	}
	
	public void OnDrawGizmos(){
		
		if (path != null){
			
			for (int i = targetIndex; i < path.Length; i++){
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one/3);
				
				if (i == targetIndex){
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1], path[i]);
				}
			}
		}
	}

	public void TriggerHitAnimation(bool right){

		if (right){
			anim.SetTrigger("Hit Right");
		}
		else{
			anim.SetTrigger ("Hit Left");
		}
	}

	/*void OnCollisionEnter2D(Collision2D  collision) 
	{

		if(collision.gameObject.tag == "projectile")
		{ 
			Collider2D collider = collision.collider;
			Vector3 contactPoint = collision.contacts[0].point;
			Vector3 center = collider.bounds.center;
			
			bool right = contactPoint.x > center.x;
			//bool top = contactPoint.y > center.y;

			if (right){
				anim.SetTrigger("Hit Right");
			}
			else{
				anim.SetTrigger ("Hit Left");
			}
		}

	}*/
}
