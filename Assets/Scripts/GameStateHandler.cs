using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameStateHandler : MonoBehaviour {

	private Texture2D GameLogo;
	private Material currentMat;
	private Texture SpaceCashIcon; 
	private int spaceCash;

	//public List<GameObject>
	public int GetSpaceCash() {
		return spaceCash;
	}
	public void SetSpaceCash(int adj) {
		spaceCash += adj;
	}

	public enum PlanetType { Warm, Habitable, Cold };
		private PlanetType _planetType;

	public void SetCurrentPlanetType(PlanetType type) {
		_planetType = type;
	}
	public PlanetType GetCurrentPlanetType() {
		return _planetType;
	}

	public MissionType currentMissionType {get; private set;}
	public Dictionary<string, GameObject> MissionName = new Dictionary<string, GameObject>();
	public string currentPlanetName;
	public List<GameObject> planets = new List<GameObject>();
	public List<string> PlanetMissionCompleted = new List<string>();


	public Material GetCurrentMat() {
		return currentMat;
	}
	public void SetCurrentMat(Material mat) {
		currentMat = mat;
	}

	public void SetCurrentMission(MissionType type){
		currentMissionType = type;

	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (transform);
		SpaceCashIcon = Resources.Load ("spaceCash_icon") as Texture;
		GameLogo = Resources.Load("textLogo") as Texture2D;
		StartCoroutine(fillMissionName());

	}

	IEnumerator fillMissionName(){

		yield return null;
		foreach(GameObject planet in planets){
			MissionName.Add (planet.name, planet);
			//print(planet.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Z)){
			print (PlanetMissionCompleted.Count);
		}
	
	}

	public void SetPlanetMissionState(){

		planets.Clear();
		planets = GameObject.FindGameObjectsWithTag("Planet").ToList();

		foreach(GameObject planet in planets){
			if (PlanetMissionCompleted.Count > 0){
				if (PlanetMissionCompleted.Contains(planet.name)){
					planet.GetComponent<MissionHandler>().completed = true;
				}
			}
		}
	}

	IEnumerator ChangeLevel(int level) {
		float fadeTime = GameObject.Find ("SceneFader").GetComponent<Fading>().BegindFade(1);
		yield return new WaitForSeconds (fadeTime);
		Application.LoadLevel (level);
	}
	void OnGUI() {
		if (Application.loadedLevel == 0) {

			GUI.DrawTexture (new Rect(Screen.width/2-GameLogo.width/2, Screen.height/2-200, GameLogo.width, GameLogo.height), GameLogo);

//			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-50, 200, 100), "Explore a new galaxy"))
//				StartCoroutine(ChangeLevel(1));
//
//			if(GUI.Button (new Rect(Screen.width/2-100, Screen.height/2+60, 200, 50), "Options"))
//				Debug.Log ("you ain't got no motherfucking options bitch!");	

		}

		if (Application.loadedLevel == 1 || Application.loadedLevel == 2) {
			GUI.Label (new Rect(280, 5, 100, 25), spaceCash.ToString ());
			GUI.DrawTexture (new Rect(250, 5, 25, 25), SpaceCashIcon);
		}

	}

	public void StartGame (int level) {
		StartCoroutine(ChangeLevel(1));
	}
	
	public void QuitGame () {
		Debug.Log("Quitting Game");
		Application.Quit();
	}
}
