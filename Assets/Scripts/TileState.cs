using UnityEngine;
using System.Collections;

public class TileState : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	
		int rand = Random.Range(0, 100);
		if (rand < 15){
			Vector3 tmp = transform.position;
			transform.position = new Vector3(tmp.x,tmp.y+.1f,tmp.z);
			gameObject.layer = 8; // unwalkable
			gameObject.GetComponent<Renderer>().material.color = Color.red;
		}
		else if (rand >= 15 && rand < 50){
			gameObject.layer = 10; // unwalkable
			gameObject.GetComponent<Renderer>().material.color = Color.green;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
