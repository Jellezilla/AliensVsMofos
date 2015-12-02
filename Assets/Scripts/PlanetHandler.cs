using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetHandler : MonoBehaviour {


	private bool confirmation;
	Rect windowRect;
	// Use this for initialization
	private float oxygenLevel;
	GameStateHandler.PlanetType type;
	public int planetIndex;
	public int sunIndex;
	public int[,] planetMap;
	TileHandler th; 
	int enemyCount;

	//List<GameObject> enemies = new List<GameObject>();

	public float GetOxygenLevel() {
		return oxygenLevel;
	}
	public void SetOxygenLevel(float adj) {
		oxygenLevel += adj;
	}

	void Start () {
		enemyCount = Random.Range (3,6);
		th = GameObject.Find ("TileHandler").GetComponent<TileHandler> ();
		confirmation = false;
		windowRect = new Rect(Screen.width/2-200, Screen.height/2-100, 400,200);
		SpawnEnemies();


//		SpawnPlayer ();
	}

	void SpawnEnemies(){

		for (int i = 0; i < enemyCount; i++){
			Tile tile = th.GetWalkableTile();
			GameObject.Instantiate(Resources.Load("Alien") as GameObject, tile.transform.position, tile.transform.rotation);
		}

	}

	public void SavePlanet() {
		planetMap = new int[th.rows, th.columns];
	}


	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
		if(GUI.Button(new Rect(Screen.width-215, 15, 200, 35), "Leave Planet")) {
			confirmation = true;
		}
		if(confirmation) {
			
			Rect WindowRect = GUI.Window(0,windowRect, ConfirmWindow, "Confirm leaving Planet");
		}
	}
	void ConfirmWindow(int windowID) {
		GUI.TextArea (new Rect (15, 20, 370, 100), "You are about to leave this Planet. Once you do, you will never be able to return to this place again.\nAny structures built will be abandoned. \n\nAre you sure you want to leave this Planet?");
		if(GUI.Button (new Rect(100, 150, 85, 20), "Yes")) {
			StartCoroutine(ChangeLevel(Application.loadedLevel-1));
		}
		if(GUI.Button (new Rect(215, 150, 85, 20), "No")) {
			confirmation = false;
		}
	}
	IEnumerator ChangeLevel(int level) {
		float fadeTime = GameObject.Find ("SceneFader").GetComponent<Fading>().BegindFade(1);
		yield return new WaitForSeconds (fadeTime);
		Application.LoadLevel (level);
	}

}
