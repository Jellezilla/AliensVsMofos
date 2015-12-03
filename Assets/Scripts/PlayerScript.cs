using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayerScript : MonoBehaviour {

	public float speed;
	// The health bar.
	public RectTransform healthTransform;
	public RectTransform oxygenTransform;

	public Canvas canvas;
	Rect windowRect;
	// Storing the Y position.
	private float cachedHealthY;
	private float minHealthXValue;
	private float maxHealthXValue;

	private float cachedOxygenY;
	private float minOxygenXValue;
	private float maxOxygenXValue;

	private float currentHealth;
	private float currentOxygen;
	public bool onAirArea = false;
	public float chargeDownSpeed = 1f;
	public float chargeUpSpeed = 10f;
	GameStateHandler gsh;
	AmmunitionVariables ammoVariables;
	Gun gun;
	ShopMenuManager shopMenuManager;
	public Texture terraformerIcon;
	private bool terraFormerPurchased = false;
	GameObject TerraPrefab;
	Text cashText;
	bool confirmation;
	// Everytime we access the CurrentHealth property, which changes the health, then the "HandleHealth" method is called, which adjusts the position and color of the health bar
	private float CurrentHealth {
		get {return currentHealth;}
		set {currentHealth = value;
			HandleHealth();
		}
	}

	private float CurrentOxygen {
		get {return currentOxygen;}
		set {currentOxygen = value;
			HandleOxygen();
		}
	}
	public float maxHealth = 100;
	public float maxOxygen = 100;

	public Image visualOxygen;
	public Image visualHealth;

	private bool guiShow;
	//public float spaceCash;

	//public int cash = 0;
//	public int maxHealth = 100;
//	public int currentHealth {get; private set;}
	public bool alive {get; private set;}
	bool respawned = false;
	
	public int damage = 40;
	public float criticalChange = 20;
	public float criticalMultiplier = 2.5f;
	
	public Mission currentMission;
	public List<Mission> activeMissions = new List<Mission>();
	public List<Mission> completedMissions = new List<Mission>();

	public bool shopMenuOpen = false;


	//PlayerController otherPlayerScript;

	// Use this for initialization
	void Start () {
		alive = true;
		windowRect = new Rect(Screen.width/2-200, Screen.height/2-100, 400,200);

		// Storing the Y position of the health bar.
		cachedHealthY = healthTransform.position.y;
		cachedOxygenY = oxygenTransform.position.y;
		// Storing the X value of the bar at max health, which is the starting position of the bar.
		maxHealthXValue = healthTransform.position.x;
		maxOxygenXValue = oxygenTransform.position.x;

		// The x position of the bar at minimum health is the starting position of the bar minus the width of the rectangle (the health bar).
		minHealthXValue = healthTransform.position.x - healthTransform.rect.width;
		minOxygenXValue = oxygenTransform.position.x - oxygenTransform.rect.width;

		currentHealth = maxHealth;
		currentOxygen = maxOxygen;

		cashText = GameObject.FindGameObjectWithTag("SpaceCash").GetComponent<Text>() as Text;
		gsh = GameObject.Find("GameStateHandler").GetComponent<GameStateHandler>();
		ammoVariables = GetComponent<AmmunitionVariables>();
		gun = GetComponent<Gun>();
		shopMenuManager = GameObject.FindGameObjectWithTag("Canvas2").GetComponent<ShopMenuManager>();
		//spaceCash = gsh.GetSpaceCash ();
	}

	public void ApplyDamage(int dmg){
		if (CurrentHealth > 0)
			CurrentHealth -= dmg;

		if (CurrentHealth <= 0){
			alive = false;
		}
	}


	IEnumerator Respawn() {
		float fadeTime = GameObject.Find ("SceneFader").GetComponent<Fading>().BegindFade(1);
		respawned = true;
		yield return new WaitForSeconds (fadeTime);
		alive = true;
		Application.LoadLevel (1);
	}
	
	// Update is called once per frame
	void Update () {
		//HandleMovement();
		if (currentOxygen <= 0)
			alive = false;

		cashText.text = "Cash: "+gsh.GetSpaceCash().ToString();

		if (!alive && !respawned){
			StartCoroutine(Respawn());
		}

		if(!onAirArea && currentHealth > 0 && !terraFormerPurchased) {
			CurrentOxygen -= (1f * Time.fixedDeltaTime) * chargeDownSpeed;
		}

		// Weapon Input
		if (Input.GetMouseButton(0)) {
			GetComponent<GunController>().OnTriggerHold();
			//gunController.OnTriggerHold();
		}

		if (Input.GetMouseButtonUp(0)) {
			GetComponent<GunController>().OnTriggerRelease();
		}

		if (guiShow == true) {

		/*	if(Input.GetKeyDown("q") && shopMenuOpen == false) {
				shopMenuManager.ShowMenu(shopMenuManager.CurrentMenu);
				shopMenuOpen = true;
			}
			else if (Input.GetKeyDown("q") && shopMenuOpen == true) {
				shopMenuManager.CloseMenu(shopMenuManager.CurrentMenu);
				shopMenuOpen = false;
			}*/
		}
	}

	private void HandleHealth() {
		// Contains the X position of the health bar.
		float currentXValue = MapValues (currentHealth, 0, maxHealth, minHealthXValue, maxHealthXValue);

		healthTransform.position = new Vector3 (currentXValue, cachedHealthY);

		if (currentHealth > maxHealth / 2) { // If health is more than 50%
			visualHealth.color = new Color32((byte)MapValues(currentHealth, maxHealth / 2, maxHealth, 255, 0), 255, 0, 255);
			//GetComponent<AudioSource>().loop = true;
			//GetComponent<AudioSource>().Play();
		}
		else { //If health is less than 50%
			visualHealth.color =  new Color32(255, (byte)MapValues(currentHealth, 0, maxHealth/2, 0, 255), 0, 255);
		}

		if (currentHealth == maxHealth) {
			GetComponent<AudioSource>().Stop();
		}
	}


	void OnTriggerStay(Collider other) {

		if (other.tag == "Air") {
			// If we're not on cooldown and health is greater than "0", then cooldown.
			if(currentOxygen < maxOxygen) {

				CurrentOxygen += (1 * Time.fixedDeltaTime) * chargeUpSpeed;
			}
		}

		if(other.tag == "Shop") {

			if (!shopMenuOpen){
				shopMenuManager.ShowMenu(shopMenuManager.CurrentMenu);
				shopMenuOpen = true;
			}
			guiShow = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.tag == "Shop") {
			guiShow = false;
			if (shopMenuOpen){
				shopMenuManager.CloseMenu(shopMenuManager.CurrentMenu);
				shopMenuOpen = false;
			}
		}
		if(other.tag == "Air") {
			onAirArea = false;
		}
	}

	private void HandleOxygen() {
		// Contains the X position of the health bar.
		float currentXValue = MapValues (currentOxygen, 0, maxOxygen, minOxygenXValue, maxOxygenXValue);
		
		oxygenTransform.position = new Vector3 (currentXValue, cachedOxygenY);
		
		if (currentOxygen > maxOxygen / 2) { // If health is more than 50%
			visualOxygen.color = new Color32((byte)MapValues(currentOxygen, maxOxygen / 2, maxOxygen, 255, 0), 255, 0, 255);
			//GetComponent<AudioSource>().loop = true;
			//GetComponent<AudioSource>().Play();
		}
		else { //If health is less than 50%
			
			visualOxygen.color =  new Color32(255, (byte)MapValues(currentOxygen, 0, maxOxygen/2, 0, 255), 0, 255);
		}
		
		if (currentOxygen == maxOxygen) {
			GetComponent<AudioSource>().Stop();
		}
	}

	public void BuySpaceSuit() {
		if(gsh.GetSpaceCash() >= 110) {
			Debug.Log("You clicked 'Buy lighter space suit'");
			gsh.SetSpaceCash(-110);
			gameObject.GetComponent<Movement>().maxSpeed = 3;
		}
	}
	
	public void BuyOxygen() {
		if(gsh.GetSpaceCash() >= 80) {
			Debug.Log("You clicked 'Buy bigger oxygen tank'");
			gsh.SetSpaceCash(-80);
			chargeDownSpeed = .6f;
		}
	}
	
	public void BuyRevolver() {
		if(gsh.GetSpaceCash() >= 1000) {
			gsh.SetSpaceCash(-1000); //-= 1000;
			gameObject.GetComponent<GunController>().BuyRevolver();
		}
	}
	
	public void BuyRifle() {
		if(gsh.GetSpaceCash() >= 1500) {
			gsh.SetSpaceCash(-1500);
			gameObject.GetComponent<GunController>().BuyRifle();
		}
	}
	
	public void BuyShotgun() {
		if(gsh.GetSpaceCash() >= 1700) {
			gsh.SetSpaceCash(-1700);
			gameObject.GetComponent<GunController>().BuyShotgun();
		}
	}
	
	public void BuyTerraformer() {
		if(gsh.GetSpaceCash() >= 2500) {
			gsh.SetSpaceCash(-2500);
			terraFormerPurchased = true;
		}
	}
	
	public void BuyRevolverAmmo() {
		if(gsh.GetSpaceCash() >= 20) {
			gsh.SetSpaceCash(-20);
			ammoVariables.revolverCurrentAmmo += 100;
		}
	}
	
	public void BuyRifleAmmo() {
		if(gsh.GetSpaceCash() >= 30) {
			gsh.SetSpaceCash(-30);
			ammoVariables.rifleCurrentAmmo += 100;
		}
	}
	
	public void BuyShotgunAmmo() {
		if(gsh.GetSpaceCash() >= 40) {
			gsh.SetSpaceCash(-40);
			ammoVariables.shotgunCurrentAmmo += 100;
		}
	}
	
	public void ExitShopMenu() {
		shopMenuManager.CloseMenu(shopMenuManager.CurrentMenu);
		shopMenuOpen = false;
	}


	void OnGUI() {
		if(guiShow == true) {

//			if(Input.GetKeyDown("q") && shopMenuOpen == false) {
//				shopMenuManager.ShowMenu(shopMenuManager.CurrentMenu);
//				shopMenuOpen = true;
//			}
//			else if (Input.GetKeyDown("q") && shopMenuOpen == true) {
//				shopMenuManager.CloseMenu(shopMenuManager.CurrentMenu);
//				shopMenuOpen = false;
//			}

	/*
			GUI.Box(new Rect(10, 60, 220, 600), "Buy items");
			GUI.Label(new Rect(65, 630, 220, 35), "$pace Ca$h: " + gsh.GetSpaceCash());
			//GUI.Button shotgunButton = GUI.Button(new Rect(10, 310, 200, 30), "Buy shotgun");
			if(GUI.Button(new Rect(20, 100, 200, 30), "Buy lighter space suit $110,00")) {
				if(gsh.GetSpaceCash() >= 110) {
					Debug.Log("You clicked 'Buy lighter space suit'");
					gsh.SetSpaceCash(-110);
					gameObject.GetComponent<Movement>().maxSpeed = 3;
				}
			}

			if(GUI.Button(new Rect(20, 160, 200, 30), "Buy bigger oxygen tank $80,00")) {
				if(gsh.GetSpaceCash() >= 80) {
					Debug.Log("You clicked 'Buy bigger oxygen tank'");
					gsh.SetSpaceCash(-80);
					chargeDownSpeed = .6f;
				}
			}

			if(GUI.Button(new Rect(20, 220, 200, 30), "Buy revolver $1.000,00")) {
				if(gsh.GetSpaceCash() >= 1000) {
					gsh.SetSpaceCash(-1000); //-= 1000;
					gameObject.GetComponent<GunController>().BuyRevolver();
				}
			}

			if(GUI.Button(new Rect(20, 280, 200, 30), "Buy assault rifle $1.500,00")) {
				if(gsh.GetSpaceCash() >= 1500) {
					gsh.SetSpaceCash(-1500);
					gameObject.GetComponent<GunController>().BuyRifle();
				}
			}

			if (GUI.Button(new Rect(20, 340, 200, 30), "Buy shotgun $1.700,00")) {
				if(gsh.GetSpaceCash() >= 1700) {
					gsh.SetSpaceCash(-1700);
					gameObject.GetComponent<GunController>().BuyShotgun();
				}
			}

			if(GUI.Button (new Rect (20, 400, 200, 30), "Buy Terraformer $2.500,00")) {
				if(gsh.GetSpaceCash() >= 2500) {
					gsh.SetSpaceCash(-2500);
					terraFormerPurchased = true;
				}
			}

			if(GUI.Button (new Rect (20, 460, 200, 30), "Buy 100 revolver rounds $20,00")) {
				if(gsh.GetSpaceCash() >= 20) {
					gsh.SetSpaceCash(-20);
					ammoVariables.revolverCurrentAmmo += 100;
				}
			}

			if(GUI.Button (new Rect (20, 520, 200, 30), "Buy 100 rifle rounds $30,00")) {
				if(gsh.GetSpaceCash() >= 30) {
					gsh.SetSpaceCash(-30);
					ammoVariables.rifleCurrentAmmo += 100;
				}
			}

			if(GUI.Button (new Rect (20, 580, 200, 30), "Buy 100 shotgun rounds $40,00")) {
				if(gsh.GetSpaceCash() >= 40) {
					gsh.SetSpaceCash(-40);
					ammoVariables.shotgunCurrentAmmo += 100;
				}
			}*/
		}


		if (terraFormerPurchased) {
			GUI.DrawTexture (new Rect(Screen.width-50,Screen.height/2, 50, 50), terraformerIcon);
			if(GUI.Button (new Rect(Screen.width-50, Screen.height/2, 50, 50), "")) {
				//GUI.DrawTexture(new Rect(Input.mousePosition.x, Input.mousePosition.y, 50, 50), terraformerIcon);
				TileHandler th = GameObject.Find ("TileHandler").GetComponent<TileHandler>();
				if(!th.GetTile((int)transform.position.x,(int)transform.position.z).blocked &&
			       !th.GetTile((int)transform.position.x+1,(int)transform.position.z).blocked &&
				   !th.GetTile((int)transform.position.x,(int)transform.position.z+1).blocked &&
			       !th.GetTile((int)transform.position.x+1,(int)transform.position.z+1).blocked) {
				 	TerraPrefab = (GameObject)Resources.Load("TerraFormer");
					Instantiate(TerraPrefab, new Vector3(transform.position.x+.5f,0.0f,transform.position.z+.5f),Quaternion.identity);
					terraFormerPurchased = false;
				}

			}
		}

		if (guiShow){
			if(GUI.Button(new Rect(Screen.width-215, 15, 200, 35), "Leave Planet")) {
				confirmation = true;
			}
			if(confirmation) {
				
				Rect WindowRect = GUI.Window(0,windowRect, ConfirmWindow, "Confirm leaving Planet");
			}
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

	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax) {
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
}
