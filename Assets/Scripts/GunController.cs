using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GunController : MonoBehaviour {

	public Transform weaponHold;
	//public Gun startingGun;

	// Next line is from older tutorial. And everything that's commented out is stuff from new tutorial.
	public Gun[] guns;
	//public Gun[] purchasedGuns;
	public List<Gun> purchasedGuns = null;

	//private bool reloading;

	//private AmmoGUI gui;

//	public float currentAmmo;
//	public float currentClip;
//	public float maxAmmo;
//	public float maxClip;



	//public Rect guiAreaRect = new Rect(0,0,0,0);

	public Gun equippedGun;

	public void BuyRevolver() {
		purchasedGuns.CopyTo(0, guns, 0, 1);
		EquipGun(0);

	}

	public void BuyRifle() {
		purchasedGuns.CopyTo(1, guns, 1, 1);
		EquipGun(1);
	}

	public void BuyShotgun() {
		purchasedGuns.CopyTo(2, guns, 2, 1);
	}

	void Start() {
		//purchasedGuns.CopyTo(0, guns, 0, 1);
		/*if (startingGun != null) {*/
			//EquipGun(/*startingGun*/ 0);
		/*}*/

		//gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<AmmoGUI>();
	}


	void Update() {

		for (int i = 0; i < guns.Length; i++) {
			if (Input.GetKeyDown((i+1).ToString()) && guns[i] != null /*|| Input.GetKeyDown("{" + (i+1) + "}")*/) {
				EquipGun(i);
				break;
			}
		}

//		if(Input.GetKeyDown(KeyCode.R)) {
//			if (equippedGun.Reload()) {
//				reloading = true;
//			}
//		}
//
//		if (reloading) {
//			equippedGun.FinishReload();
//			reloading = false;
//		}

	}

	public void EquipGun(int i /*Gun gunToEquip*/) {
		if (equippedGun != null) {
			Destroy(equippedGun.gameObject);
		}

		// The gun will be instantiated on the "Weapon Hold" game object.
		equippedGun = Instantiate(/*gunToEquip*/ guns[i], weaponHold.position, weaponHold.rotation) as Gun;

		// Making the gun a child of "Weapon Hold", so it moves with the player.
		equippedGun.transform.parent = weaponHold;

		//equippedGun.gui = gui;
	}



	public void OnTriggerHold() {
		// Checking if there's a weapon currently equipped.
		if (equippedGun != null) {
			//print (equippedGun.name);
			equippedGun.OnTriggerHold();
		}
	}

	public void OnTriggerRelease() {
		// Checking if there's a weapon currently equipped.
		if (equippedGun != null) {
			equippedGun.OnTriggerRelease();
		}
	}
}
