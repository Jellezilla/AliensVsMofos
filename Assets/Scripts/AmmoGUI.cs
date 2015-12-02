using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AmmoGUI : MonoBehaviour {

	public Text ammoText;

	public void SetAmmoInfo(int totalAmmo, int ammoInMag) {
		ammoText.text = ammoInMag + "/" + totalAmmo;
	}

}
