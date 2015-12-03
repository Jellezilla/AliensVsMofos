using UnityEngine;
using System.Collections;

// This script is responsable for setting what current menu is open.

public class ShopMenuManager : MonoBehaviour {

	public ShopMenu CurrentMenu;

	public void Start() {
		//ShowMenu(CurrentMenu);
	}



	public void ShowMenu(ShopMenu menu) {
		if (CurrentMenu != null)
			CurrentMenu.IsOpen =  false;
		
		CurrentMenu = menu;
		CurrentMenu.IsOpen = true;
		
	}

	public void CloseMenu(ShopMenu menu) {
		if (CurrentMenu != null)
			CurrentMenu.IsOpen =  true;
		
		CurrentMenu = menu;
		CurrentMenu.IsOpen = false;
		
	}
}
