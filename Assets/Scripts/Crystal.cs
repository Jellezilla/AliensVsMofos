using UnityEngine;
using System.Collections;

public class Crystal : MonoBehaviour {


	private int amount;
	private GameStateHandler gsh;


	void Start() {
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler> ();
		SetAmount (Random.Range (333, 667));

	}
	public int GetAmount() {
		return amount;
	}


	public void SetAmount(int adj) {
		amount += adj;
		if (amount <= 0) {
			Destroy(gameObject);
		} 
	}
	void OnCollisionEnter(Collision col) {
		if (col.transform.tag == "Player") {
			gsh.SetSpaceCash(amount);
			Destroy (gameObject);
		}
	}
}
