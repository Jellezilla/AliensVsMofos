using UnityEngine;
using System.Collections;

public class MissionHandler : MonoBehaviour {

	public MissionType missionType;
	public bool completed = false;
	public bool removed;
	// Use this for initialization
	void Start () {
	
		int rand = Random.Range(1,100);
		if (rand <= 25){
			missionType = MissionType.Elimination;
		}
		else if (rand > 25 && rand <= 50){
			missionType = MissionType.Intel;
		}
		else {
			missionType = MissionType.None;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (completed && !removed){
			removed  = true;
			print (gameObject.name + "completed" );
		}
	
	}
}
