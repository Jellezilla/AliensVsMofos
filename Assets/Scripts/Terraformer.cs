using UnityEngine;
using System.Collections;

public class Terraformer : MonoBehaviour {
	PlanetHandler ph; 
	// Use this for initialization
	void Start () {
		ph = GameObject.Find ("PlanetHandler").GetComponent<PlanetHandler> ();
		ph.SetOxygenLevel (100);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
