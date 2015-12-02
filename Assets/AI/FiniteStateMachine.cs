using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Patrol))]
[RequireComponent(typeof (EnemyAttributes))]


public class FiniteStateMachine : MonoBehaviour {
	
	public enum State{Patrol, Chase, Attack, Resign, Dead};
	
	Patrol patrol;
	public State state;
	Transform player;
	float chaseDistance = 10f;
	float attackDistance = 7;
	Vector3 lastKnownPlayerPos;
	Animator anim;
	EnemyAttributes enemy;
	bool setAttackOnce = true;
	// Use this for initialization
	void Start () {
		enemy = GetComponent<EnemyAttributes>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		state = State.Patrol;
		patrol = GetComponent<Patrol>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if (enemy.alive){
			float distanceToPlayer = Vector3.Distance(transform.position, player.position);
			if (distanceToPlayer <= chaseDistance){
							
				RaycastHit hit;
				if(Physics.Raycast(transform.position, (player.position-transform.position).normalized, out hit, chaseDistance)){		
					Debug.DrawRay(transform.position, (player.position-transform.position).normalized*chaseDistance, Color.green);		
				}
				if (distanceToPlayer > attackDistance){
				state = State.Chase;
				}
				else {
					state = State.Attack;
				}
			}
			
			else {
				if (state == State.Chase && state == State.Attack)
					state = State.Resign;
			}
		}
		else {
			state = State.Dead;
		}

		switch(state){
		case State.Attack:
			enemy.StopPathFollowing();
			patrol.shouldPatrol = false;
			transform.forward = player.position - transform.position;
		//	transform.Translate(Vector3.forward * 1 * Time.deltaTime);
			if (setAttackOnce)
				StartCoroutine(AttackPlayer());
			break;
		case State.Patrol:
			StartCoroutine(setPatrol());
			break;
		case State.Resign:
			patrol.shouldPatrol = true;
			patrol.ResumePatrolling();
			break;
		case State.Chase:
			enemy.StopPathFollowing();
			patrol.shouldPatrol = false;
			transform.forward = player.position - transform.position;
			transform.Translate(Vector3.forward * 2 * Time.deltaTime);
			break;
		case State.Dead:
			StopCoroutine(AttackPlayer());
			Vector3 tmp = transform.position;
			transform.position = tmp;
			anim.SetBool("Alive", false);
			break;
		}

		if (state != State.Attack){
			setAttackOnce = true;
		}
	}

	IEnumerator AttackPlayer(){
		setAttackOnce = false;
		int numOfShots = Random.Range(3,6);
		float delay = Random.Range (0.4f,2f);
		IEnumerator shoot = enemy.ShootAtTarget(player.position, numOfShots);
		yield return StartCoroutine(shoot);
		yield return new WaitForSeconds(delay);

		StartCoroutine(AttackPlayer());
	}

	IEnumerator setPatrol(){
		yield return new WaitForSeconds(.5f);
		patrol.shouldPatrol = true;
	}
}