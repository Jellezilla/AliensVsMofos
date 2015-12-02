using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

	//public GameObject Player;
	//public AudioSource geigerSound;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider otherObj)
	{
		if (otherObj.tag == "Player")
		{
			GetComponent<AudioSource>().Play();
		}
		else {
			GetComponent<AudioSource>().Stop();
		}
	}

	void OnTriggerExit(Collider otherObj)
	{
		if (otherObj.tag == "Player")
		{
			GetComponent<AudioSource>().Stop();
		}
	}
}
