using UnityEngine;
using System.Collections;

public class GameStateHandler : MonoBehaviour {

	private Texture2D GameLogo;
	private Material currentMat;

	private int spaceCash;

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

	public Material GetCurrentMat() {
		return currentMat;
	}
	public void SetCurrentMat(Material mat) {
		currentMat = mat;
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (transform);
		GameLogo = Resources.Load("textLogo") as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	IEnumerator ChangeLevel(int level) {
		float fadeTime = GameObject.Find ("SceneFader").GetComponent<Fading>().BegindFade(1);
		yield return new WaitForSeconds (fadeTime);
		Application.LoadLevel (level);
	}
	void OnGUI() {
		if (Application.loadedLevel == 0) {

			GUI.DrawTexture (new Rect(Screen.width/2-GameLogo.width/2, Screen.height/2-200, GameLogo.width, GameLogo.height), GameLogo);

			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-50, 200, 100), "Explore a new galaxy"))
				StartCoroutine(ChangeLevel(1));

			if(GUI.Button (new Rect(Screen.width/2-100, Screen.height/2+60, 200, 50), "Options"))
				Debug.Log ("you ain't got no motherfucking options bitch!");	

		}

		if (Application.loadedLevel == 1 || Application.loadedLevel == 2) {
			GUI.Label (new Rect(250, 5, 150, 25), "SpaceCash: "+spaceCash);
		}

	}
}
