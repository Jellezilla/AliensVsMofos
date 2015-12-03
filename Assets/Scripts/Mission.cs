using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum MissionType{ Intel, Elimination, None };
public enum MissionRewardType{ Money };

public class Mission : MonoBehaviour {

	public MissionType missionType;
	public MissionRewardType rewardType;
	public bool completed {get; protected set;}
	protected bool rewardGiven = false;
	protected PlayerScript playerScript;
	private int rewardValue = 3000;
	private Transform target;
	private GameStateHandler gsh;
	private TileHandler th;



	//protected Player

	// Use this for initialization
	public void Awake () {
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler>();
		missionType = gsh.currentMissionType;
		//target = GameObject.FindGameObjectWithTag("MissionTarget").transform;
		//playerAttributes = GameObject.FindWithTag("Player").transform.GetComponent<PlayerAttributes>();

		playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
		playerScript.activeMissions.Add(this);
		playerScript.currentMission = this;
	}

	public void Start(){
		th = GameObject.FindGameObjectWithTag("TileHandler").GetComponent<TileHandler>();
		switch(missionType){

		case MissionType.Elimination:
			Tile tile = th.GetWalkableTile();
			GameObject alien = GameObject.Instantiate(Resources.Load("TargetAlien") as GameObject, tile.transform.position, tile.transform.rotation) as GameObject;
			target = alien.transform;
			break;
		case MissionType.Intel:
			Tile someTile = th.GetWalkableTile();

			GameObject intel = GameObject.Instantiate(Resources.Load("Intel") as GameObject, someTile.transform.position, someTile.transform.rotation) as GameObject;
			target = intel.transform;
			break;
		 
		}


	}

	// Update is called once per frame
	public void Update () {

		if (missionType == MissionType.Elimination || missionType == MissionType.Intel)
			CheckCompletion();
	}

	void CheckCompletion(){

		switch (missionType){

		case MissionType.Intel: 
			if (target.gameObject.GetComponent<Intel>() == null){
				return;
			}
			if (target.gameObject.GetComponent<Intel>().collected){
				completed = true;
				if (!gsh.PlanetMissionCompleted.Contains(gsh.currentPlanetName))
					gsh.PlanetMissionCompleted.Add (gsh.currentPlanetName);
				//gsh.MissionName[gsh.currentPlanetName].gameObject.GetComponent<MissionHandler>().completed = true;
				if (!rewardGiven)
					HandleReward();
			}

			break;
		case MissionType.Elimination:
			if (target != null){
				if (!target.GetComponent<EnemyAttributes>().alive){
					completed = true;
					if (!gsh.PlanetMissionCompleted.Contains(gsh.currentPlanetName))
						gsh.PlanetMissionCompleted.Add (gsh.currentPlanetName);
					if (!rewardGiven)
						HandleReward();
				}
			}
			break;
		}
	}

	protected void HandleReward(){
		playerScript.currentMission = null;
		playerScript.completedMissions.Add(this);
		playerScript.activeMissions.Remove(this);
		rewardGiven = true;
		switch(rewardType){
		case MissionRewardType.Money:
			//playerScript.spaceCash += rewardValue;
			GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler>().SetSpaceCash(rewardValue);
			break;
		}
	}


	public void OnGUI(){

		if(!completed){
			switch (missionType){
				
			case MissionType.Intel: 
				GUI.Label(new Rect(Screen.width/3, 10, 300, 20), "Find and retrieve the round intel package.");

				break;
			case MissionType.Elimination:
				GUI.Label(new Rect(Screen.width/3, 10, 300, 20), "Find and eliminate the oversized alien.");

				break;
			}
		}
		else{
			GUI.Label(new Rect(Screen.width/3, 10, 300, 20), "Mission complete. Return to your ship.");
		}

	}


}
