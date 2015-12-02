using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]

public class Intel : MonoBehaviour {
	
	Collider col; 
	public bool collected {get; private set;}
	Renderer renderer;
	// Use this for initialization
	void Start () {
		col = GetComponent<Collider>();
		renderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			collected = true;
			col.enabled = false;
			renderer.enabled = false;
		}

	}


}
