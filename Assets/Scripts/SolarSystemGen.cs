using UnityEngine;
using System.Collections;
using UnityEditor;


public class SolarSystemGen : MonoBehaviour {


	public float scaleFactor;
	private bool confirmation;
	private Rect windowRect;

	GameStateHandler gsh;
	PlayerControl pc;

	public int counter;
	public int planetCount;

	// Use this for initialization
	void Start () {
		scaleFactor = 20.0f;
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler> ();
		pc = GameObject.Find ("SpaceShip").GetComponent<PlayerControl> ();
		windowRect = new Rect(Screen.width/2-200, Screen.height/2-100, 400,200);
		confirmation = false;


		// Loop that creates the solar systems. 
		for (counter = 0; counter < 4; counter++) {
			scaleFactor = (20.0f + Random.Range (-3,4)); // set a random offset on the scaleFactor. 
			planetCount = Random.Range (5, 9); // set random number of planets for each solar system. 
			GenerateSolarSystem (planetCount, counter); // Call the method that generates a soler system. 
		}
	}
	
	// Update is called once per frame
	void Update () {


		if(Input.GetMouseButtonDown (0)) {
			Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast (ray, out hit)) {
				if(hit.transform.tag == "Planet") {
					gsh.SetCurrentMission(hit.transform.GetComponent<MissionHandler>().missionType);

					//GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler>().SetCurrentMat(hit.transform.GetComponent<Renderer>().material); // her har vi allerede noget!! Sæt en planet type istedet for at sætte mats. Planet type = enum fætter.
					Orbit orb = hit.transform.GetComponent<Orbit>();
					if(orb.distanceToStar < (20.0f * scaleFactor)) {
						gsh.SetCurrentPlanetType(GameStateHandler.PlanetType.Warm);
					} else if(orb.distanceToStar > (20.0f * scaleFactor) && orb.distanceToStar < (40.0f * scaleFactor)) {
						gsh.SetCurrentPlanetType(GameStateHandler.PlanetType.Habitable);
					} else if (orb.distanceToStar > (40.0f * scaleFactor)) {
						gsh.SetCurrentPlanetType(GameStateHandler.PlanetType.Cold);
					}	

					StartCoroutine (Warp(1.0f));
					//StartCoroutine(ChangeLevel (Application.loadedLevel+1));
				}
			}
		}
	}

	/// <summary>
	/// Raises the GU event.
	/// In this case, the button where you can choose to warp to another solar system. 
	/// </summary>
	void OnGUI() {
		if(GUI.Button(new Rect(Screen.width-215, 15, 200, 35), "Warp to new Solar System")) {
			confirmation = true;
		}
		if(confirmation) {

			Rect WindowRect = GUI.Window(0,windowRect, ConfirmWindow, "Confirm leaving solar system");
		}
	}
	void ConfirmWindow(int windowID) {
		GUI.TextArea (new Rect (15, 20, 370, 100), "You are about to leave these solar systems. Once you do, you will never be able to return to this place again.\nAny structures built will be abandoned. \n\nAre you sure you want to warp to new solar system and leave these behind?");
		if(GUI.Button (new Rect(100, 150, 85, 20), "Yes")) {
			StartCoroutine(ChangeLevel(Application.loadedLevel));
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

	public void GenerateSolarSystem(int _planetCount, int index) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = GetSunPos (index); //new Vector3(15.0F, 2.5F, 15.0F);

		sphere.transform.eulerAngles = GetSunRot (index); // the rotation is actually being set, but it is not applied to the children of the sphere, so it doesn't really work 


		sphere.transform.SetParent (GameObject.FindWithTag("God").transform);
		sphere.transform.name = "Sun";
		Material newMat = Resources.Load("Materials/PlanetMats/sun", typeof(Material)) as Material;
		sphere.transform.localScale += new Vector3((4.0F * scaleFactor), (4.0F * scaleFactor), (4.0F * scaleFactor));
		sphere.GetComponent<Renderer> ().material = newMat;

		for (int i = 1; i <= _planetCount; i++) {
			// Instantiate planet (sphere)
			GameObject planet = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			// Set the initial position of the new planet
			planet.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y, sphere.transform.position.z);
			planet.AddComponent<MissionHandler>();
			// Set name and parent of new planet
			planet.name = ("planet"+i.ToString()+"-"+Random.value);
			gsh.planets.Add (planet);

		//	planet.AddComponent<MissionHandler>();
			planet.transform.SetParent (sphere.transform);
			planet.tag = "Planet";
			// Add the orbit script to the new planet
			Orbit orb = planet.AddComponent<Orbit>();

			// set distance to star
			orb.distanceToStar = (6.5F*i + Random.Range (5.0F, 8.0F)) * scaleFactor; // the 6.5f value should be changed to the orb.distanceToStar of the previous planet

			// set the orbitSpeed of the new planet
			orb.orbitSpeed = ((_planetCount * 6.5F * scaleFactor) - (Random.Range (0.7F, 0.9F) * orb.distanceToStar)) / scaleFactor; 

			// set the size of the new planet
			float rand = 1.0f / scaleFactor; //Random.Range(1,6);
			planet.transform.localScale += new Vector3(rand*scaleFactor,rand*scaleFactor,rand*scaleFactor);

			// Add trail to orbiting planet
			TrailRenderer tr = planet.AddComponent<TrailRenderer>();
			tr.time = orb.distanceToStar;

			tr.startWidth = 0.5F;
			tr.endWidth = 0.1F;
			tr.material = Resources.Load ("Materials/PlanetMats/TrailMat", typeof(Material)) as Material;


			// Set rotational speed of the new planet

			// Assign a new material to the new planet
			orb.rotationSpeed = 365;

			// get random material:
			planet.GetComponent<Renderer>().material = GetRandPlanetMat(orb.distanceToStar);

			// Add moons? 
		}
		
	}

	private Vector3 GetSunRot (int index) {
		Quaternion ident = Quaternion.identity;

		if (index == 0) {
			ident.eulerAngles = new Vector3(30, 0, 30);
			//ident = new Quaternion(30,0,30,1);
		} else if (index == 1) {
			ident.eulerAngles = new Vector3(30, 0, -30);
		} else if (index == 2) {
			ident.eulerAngles = new Vector3(-30, 0, 30);
		} else if (index == 3) {
			ident.eulerAngles = new Vector3(-30, 0, -30);
		}
		return ident.eulerAngles;
	}
	private Vector3 GetSunPos(int index) {
		Vector3 pos = Vector3.zero;
		if (index == 0) {
			pos = new Vector3(0,0,0);
		} else if (index == 1) {
			pos = new Vector3(0,0,3000);
		} else if (index == 2) {
			pos = new Vector3(3000, 0, 0);
		} else if (index == 3) {
			pos = new Vector3(3000, 0, 3000);
		}  

		return pos;
	}
	/// <summary>
	/// Returns a random material based on the distance.
	/// </summary>
	/// <param name="dist">Dist.</param>
	private Material GetRandPlanetMat(float dist) {
		Material _mat;
		int rand = Random.Range (1,3);
		_mat = Resources.Load ("Materials/PlanetMats/01LavaWorld", typeof(Material)) as Material; // for some reason it is necessary to initialize the material with an actual mat and not just "_mat = new Material()", hence this Resource.Load

		if (dist < 20.0f * scaleFactor) {
			if ( 	rand == 1) _mat = Resources.Load ("Materials/PlanetMats/01LavaWorld", typeof(Material)) as Material;
			else if(rand == 2) _mat = Resources.Load ("Materials/PlanetMats/Mars", typeof(Material)) as Material;
			else if(rand == 3) _mat = Resources.Load ("Materials/PlanetMats/Venus", typeof(Material)) as Material;
		} else if (dist > 20.0f * scaleFactor && dist < 40.0f * scaleFactor) {
			if (	rand == 1) _mat = Resources.Load ("Materials/PlanetMats/Earth", typeof(Material)) as Material;
			else if(rand == 2) _mat = Resources.Load ("Materials/PlanetMats/Earth1", typeof(Material)) as Material;
			else if(rand == 3) _mat = Resources.Load ("Materials/PlanetMats/Earth2", typeof(Material)) as Material;
		} else if (dist > 40.0f * scaleFactor) {
			if ( 	rand == 1) _mat = Resources.Load ("Materials/PlanetMats/Mercury", typeof(Material)) as Material;
			else if(rand == 2) _mat = Resources.Load ("Materials/PlanetMats/Jupiter", typeof(Material)) as Material;
			else if(rand == 3) _mat = Resources.Load ("Materials/PlanetMats/Saturn", typeof(Material)) as Material;
		}
		return _mat;
	}

	IEnumerator Warp(float time) {
		if (true) {
			pc.currrentSpeed = pc.maxSpeed;
			pc.MaxTurbines (0.65f);
		}
		yield return new WaitForSeconds (time);
		StartCoroutine(ChangeLevel (Application.loadedLevel+1));
	}

	/*
	[CustomEditor(typeof(SolarSystemGen))]
	class SunSystemBuilder : Editor {
		SolarSystemGen ssg = GameObject.FindWithTag ("God").GetComponent<SolarSystemGen>();  // new SolarSystemGen();
	

		public override void OnInspectorGUI() {
			if (GUILayout.Button ("I am God!")) {
				ssg.counter++;
			
				//ssg.GenerateSolarSystem(ssg.planetCount, ssg.counter);
				ssg.GenerateSolarSystem(8, 4);
				Debug.Log("It's alive: " + target.name);
			}

		}
	}*/
}
