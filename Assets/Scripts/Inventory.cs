/*using UnityEngine;
using System.Collections;
//Required if you will be using lists and we will.
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public List<Weapon> myWeaponList = null;
	public Weapon currentWeapon = null;
	//We set all the values in a rect to 0 by default and we will set them in the inspector. After that let's test out how it works.
	public Rect guiAreaRect = new Rect(0,0,0,0);
	public Transform weaponPos = null;
	public GameObject currentWeaponGO = null;

	List<KeyCode> numberss = new List<KeyCode>{ KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5};


	void OnGUI()
	{
		//begin guilayout area
		GUILayout.BeginArea(guiAreaRect);
		//begin vertical group. This means that everything under this will go from up to down
		GUILayout.BeginVertical();
		//loop throught the list of out weapons
		foreach (Weapon weapon in myWeaponList) {

				//Do a gui button for every weapon we found on our weapon list. And make them draw their weaponLogos.
				if(GUILayout.Button(weapon.weaponLogo,GUILayout.Width(50),GUILayout.Height(50)))
				{

				//if we clicked the button it will but that weapon to our selected(equipped) weapon
					currentWeapon = weapon;

					ChangeWeapon(weapon);
					Debug.Log(currentWeapon);
				}
			
		}
		//We need to close vertical gpr and gui area group.
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}


	public void ChangeWeapon(Weapon newWeapon)
	{
		//if we have weapon destroy it
		if(currentWeaponGO != null)
			Destroy (currentWeaponGO);
		
		//instantiate new weapon prefab to game. it needs to be casted to be a gameobject
		currentWeaponGO = GameObject.Instantiate(newWeapon.gameObject,Vector3.zero,Quaternion.identity) as GameObject;
		//set weaponslot to be the parent of the weapon
		currentWeaponGO.transform.parent = weaponPos;
		//set weapon position to weaponslot position
		currentWeaponGO.transform.position = weaponPos.position;
		//turn weapon so it has right rotation ( not always needed )
		//currentWeaponGO.transform.localRotation = Quaternion.Euler(75f,75f,75f);


	}


	public void ChangeWeaponWithNumbers()
	{
		//loop throught our keycode list
		for(int i =0;i<numberss.Count;i++)
		{
			//check if there is something before we instantiate it
			if(myWeaponList[i] != null)
			{
				//check if whichever index keycode we have pressed
				if(Input.GetKeyDown(numberss[i]))
				{
					//set our currentWeapon to be same in our inventory as it is in our keycode list.
					currentWeapon = myWeaponList[i];
					// call weapon spawn function with that weapon.
					ChangeWeapon(myWeaponList[i]);
				}
			}
		}
	}





	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ChangeWeaponWithNumbers();


	}
}
*/