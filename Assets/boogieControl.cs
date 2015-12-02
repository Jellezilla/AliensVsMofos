using UnityEngine;
using System.Collections;

public class boogieControl : MonoBehaviour {

	[HideInInspector]
	public Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown ("b"))
		{
			anim.Play("boogie",-1,0f);
		}

		if(Input.GetMouseButtonDown(0))
		{
			anim.Play ("shoot_rifle_run",-1,0f);
		}
			
	}
}
