using UnityEngine;
using System.Collections;

public class SetMissionsState : MonoBehaviour {

	public SpriteRenderer legend;
	//public  legend
	GameStateHandler gsh;
	// Use this for initialization
	void Start () {
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler>();
		StartCoroutine(WaitOneFrame());
		StartCoroutine(disableLegend());
	}
	IEnumerator WaitOneFrame(){
		yield return null;
		gsh.SetPlanetMissionState();
	}

	IEnumerator disableLegend(){

		yield return new WaitForSeconds(5f);
		legend.enabled = false;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
