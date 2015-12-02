using UnityEngine;
using System.Collections;

// This script is responsable for setting what current menu is open.

public class MenuManager : MonoBehaviour {

	public Menu CurrentMenu;

	public void Start() {
		ShowMenu(CurrentMenu);
	}

	public void ShowMenu(Menu menu) {
		if (CurrentMenu != null)
			CurrentMenu.IsOpen =  false;
		
		CurrentMenu = menu;
		CurrentMenu.IsOpen = true;
		
	}
}
