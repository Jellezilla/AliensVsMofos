using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttributes : MonoBehaviour {

	public int cash = 0;
	public int maxHealth = 100;
	public int currentHealth {get; private set;}
	public bool alive {get; private set;}

	int damage = 40;
	float criticalChange = 20;
	float criticalMultiplier = 2.5f;

	public Mission currentMission;
	public List<Mission> activeMissions = new List<Mission>();
	public List<Mission> completedMissions = new List<Mission>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

		if (Input.GetKeyDown(KeyCode.LeftControl)){
			Shoot ();
		}
	}

	void Shoot(){
//		print ("shoot");
		RaycastHit hit;
		Vector3 direction = GetComponent<Rigidbody>().velocity.normalized;
		Debug.DrawRay(transform.position, direction*100, Color.blue);
		if(Physics.Raycast(transform.position, transform.forward, out hit, 100)){	
			if (hit.collider.tag == "Enemy"){
				float finalDamage = damage;
				int rand = Random.Range (1,100);
				if (rand <= criticalChange){
					finalDamage *= criticalMultiplier;
					//print("CRITICAL HIT");
				}
				EnemyAttributes enemyAttributes = hit.collider.gameObject.GetComponent<EnemyAttributes>();
				enemyAttributes.ApplyDamage((int)finalDamage);
				enemyAttributes.TriggerHitAnimation(true);
			}
		}
	}


}
