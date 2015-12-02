using UnityEngine;
using System.Collections;

public class Patrol : MonoBehaviour {
	
	private enum PatrolBehaviorState { MovingToNextPatrolPoint = 0, WaitingForNextMove = 1 }
	
	private Transform[] patrolPoints = new Transform[10];
	public float patrolWalkSpeed = 2.0f;
	public float delayAtPatrolPointMin = 1.0f;
	public float delayAtPatrolPointMax = 5.0f;
	FiniteStateMachine fsm;
	private PatrolBehaviorState patrolBehaviorState;
	private int patrolPointIndex;
	private float timeToWaitBeforeNextMove;
	private EnemyAttributes enemy;
	Vector3 directionVector;	
	Transform destination;
	bool pathRequested = false;
	public bool shouldPatrol = false;
	
	void Start()
	{
		enemy = GetComponent<EnemyAttributes>();
		fsm = GetComponent<FiniteStateMachine>();

		StartCoroutine(SetWayPoints());
		//print (tile.transform.position);
	}
	
	Vector3 GetCurrentDestination()
	{
		return patrolPoints[patrolPointIndex].position;
	}
	
	void ChooseNextDestination()
	{
		patrolPointIndex++;
		if (patrolPointIndex >= patrolPoints.GetLength(0))
		{
			patrolPointIndex = 0;
		}
	}
	
	IEnumerator SetWayPoints(){

		TileHandler tileHandler = GameObject.FindGameObjectWithTag("TileHandler").GetComponent<TileHandler>();

		while(!tileHandler.tileMapComplete){
			yield return null;
		}

		for (int i = 0; i < patrolPoints.Length; i++){
			Tile tile = tileHandler.GetWalkableTile();
			patrolPoints[i] = tile.transform;
		}

		print ("patrol points set");
		patrolPointIndex = 0;
		patrolBehaviorState = PatrolBehaviorState.MovingToNextPatrolPoint;
		timeToWaitBeforeNextMove = -1.0f; 
		destination = patrolPoints [patrolPointIndex+1];
		directionVector = destination.position - transform.position;

	}
	
	public void ResumePatrolling(){
		shouldPatrol = true;
		patrolBehaviorState = PatrolBehaviorState.WaitingForNextMove;
		//	UpdateMovingToNextPatrolPoint();
		UpdateWaitingForNextMove();
		fsm.state = FiniteStateMachine.State.Patrol;
		
	}
	
	void UpdateMovingToNextPatrolPoint()
	{
		Vector3 currentDestination = GetCurrentDestination();
	
		Quaternion newRot = Quaternion.LookRotation(currentDestination - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * 10f);
		//transform.forward = currentDestination - transform.position;
		//transform.Translate(Vector3.forward * patrolWalkSpeed * Time.deltaTime);
		
		if (!pathRequested){
			pathRequested = true;
			enemy.MakeRequest(currentDestination);
		}
		
		if (enemy.destinationReached){
			enemy.destinationReached = false;
			timeToWaitBeforeNextMove = Random.Range(delayAtPatrolPointMin, delayAtPatrolPointMax);
			patrolBehaviorState = PatrolBehaviorState.WaitingForNextMove;
		}
	}
	
	void UpdateWaitingForNextMove()
	{
		timeToWaitBeforeNextMove -= Time.deltaTime;
		Quaternion newRot = Quaternion.LookRotation(destination.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * 10f);
		if (timeToWaitBeforeNextMove < 0.0f)
		{
			ChooseNextDestination();
			
			if (patrolPointIndex == patrolPoints.GetLength(0)-1){
				destination = patrolPoints[0];
			}
			else if (patrolPointIndex == patrolPoints.GetLength(0)){
				destination = patrolPoints[1];
				
			}
			else{
				destination = patrolPoints[patrolPointIndex+1]; 
			}
			pathRequested = false;
			patrolBehaviorState = PatrolBehaviorState.MovingToNextPatrolPoint;
			
		}
	}
	
	void Update()
	{

		if (shouldPatrol && enemy.alive){
			if (patrolBehaviorState == PatrolBehaviorState.MovingToNextPatrolPoint)
			{
				UpdateMovingToNextPatrolPoint();
			}
			else if (patrolBehaviorState == PatrolBehaviorState.WaitingForNextMove)
			{
				UpdateWaitingForNextMove();
				
			}
		}
	}
	
	void LateUpdate()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}
}
