using UnityEngine;
using System.Collections;

public class SetMissionsState : MonoBehaviour {


	GameStateHandler gsh;
	// Use this for initialization
	void Start () {
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler>();
		StartCoroutine(WaitOneFrame());
	}
	IEnumerator WaitOneFrame(){
		yield return null;
		gsh.SetPlanetMissionState();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
