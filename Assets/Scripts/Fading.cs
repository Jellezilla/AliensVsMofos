using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000;
	private float alpha = 1.0f;
	private int fadeDir = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// fade out/in the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		alpha = Mathf.Clamp01 (alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha); // set the alpha value
		GUI.depth = drawDepth;												 // make the black texture render on top
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture); // draw the texture to fit the screen

	}

	public float BegindFade(int direction) {
		fadeDir = direction;
		return (fadeSpeed); // return the fadeSpeed variable so it's easy to time the Application.LoadLevel();
	}
	void OnLevelWasLoaded() {
		BegindFade (-1);
	}
}
