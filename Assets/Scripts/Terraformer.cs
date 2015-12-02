using UnityEngine;
using System.Collections;

public class Terraformer : MonoBehaviour {
	//PlanetHandler ph; 
	// Use this for initialization

	PlayerScript ps;

	void Start () {
		//ph = GameObject.Find ("PlanetHandler").GetComponent<PlanetHandler> ();
		//ph.SetOxygenLevel (100);
		ps = GameObject.FindWithTag ("Player").GetComponent<PlayerScript> ();
		ps.onAirArea = true;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
