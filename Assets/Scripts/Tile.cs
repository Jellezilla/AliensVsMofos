using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	TileHandler th;
    public Tile prev;
	private Material mat;
	public enum TileType { ground1, ground2, ground3, liquid, wall, unassigned };
	public TileType type;

	public bool blocked;

	float maxUpAndDown = 0.0051F;
	float speed = 200.0F;
	private float angle = 0.0F;
	private float toDegrees = Mathf.PI / 180;
	bool active;
	bool first;

    // Use this for initialization
    void Start () {
		active = false;
		first = true;
		//type = TileType.unassigned;
		mat = transform.GetComponent<Renderer> ().material;
		StartCoroutine (delayMethod (0.5F));
	}

	
	// Update is called once per frame
	void Update () {
		if (type == TileType.liquid && active) {
			angle += speed * Time.deltaTime;
			if(angle > 360) angle -= 360;
			transform.position = new Vector3(transform.position.x, transform.position.y +(maxUpAndDown * Mathf.Sin (angle * toDegrees)), transform.position.z);

		}
		


	}
	void init() {
		StartCoroutine(waitMethod((transform.position.x+transform.position.z)/10));
		//blocked = true;

	}

    public Tile getPrev()
    {
        return prev;
    }
    public void setPrev(Tile _prev)
    {
        prev = _prev;
    }
	public Material GetTileMat() {
		return mat;
	}	

	public void SetTileMat(Material _mat) {
		transform.GetComponent<Renderer> ().material = _mat;
	}

	IEnumerator delayMethod(float waitTime) {

		yield return new WaitForSeconds (waitTime);
		if (type == TileType.liquid && first == true) {
			init();
			first = false;
		}  
	}
	IEnumerator waitMethod(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		active = true;
		transform.position = new Vector3 (transform.position.x, transform.position.y - 0.1f, transform.position.z);
	}
}
