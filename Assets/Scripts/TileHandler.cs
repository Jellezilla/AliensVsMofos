
using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TileHandler : MonoBehaviour {

    public int rows, columns; 

    private int a, b;
    public Tile[,] TileMap;
	private int[,] map;
	private int[,] tmpMap;
	private Material wallMat;

	//private Transform tmp;

	public GameObject PlayerPrefab;
	public GameObject SpaceshipPrefab;
    public GameObject TilePrefab;    
	public GameObject WallPrefab;
	private GameStateHandler gsh;
	public bool tileMapComplete {get; private set;}

    // Use this for initialization
    void Start () {
		gsh = GameObject.Find ("GameStateHandler").GetComponent<GameStateHandler> ();
//		Debug.Log (gsh.GetCurrentPlanetType ().ToString ());
		SpawnTiles ();
		for (int i = 0; i < 12; i++) {
			CellularAutomata ();
		}
		SpawnSpaceship ();
		SetMaterials ();
		//StartCoroutine (wait (2.2f));
		SetColliderOnBlockedTiles ();
		for (int j = 0; j < 4; j++) {
			AddResources ();
		}
		
	
		//TilePrefab.transform.GetComponent<Tile> ().type = Tile.TileType.unassigned;
		//SpawnTileGroup ();
		//SpawnMap (0);
	}


	/// <summary>
	/// Handles input. Most of these are present for test purposes. Should be deleted in the final version. 
	/// </summary>
	void Update () {


		if (Input.GetKeyDown (KeyCode.T)) {
			CellularAutomata ();
			SetMaterials ();
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			AddResources ();
		}



	}

	public Tile GetWalkableTile(){

		int xPos = Random.Range (1, rows-1); 
		int yPos = Random.Range (1, columns-1); 
		Tile tile;

		if (!tileMapComplete){
			return null;
		}

		while (true){
			tile = GetTile (xPos,yPos);
			if (tile.blocked){
				xPos = Random.Range (1, rows-1); 
				yPos = Random.Range (1, columns-1); 
			}
			else {
				return tile;
			}
		}
	}


	/// <summary>
	/// One of the first things done, is to instantiate a set of tiles, that will be used for the CA and get materials assigned to them later on. 
	/// </summary>
    void SpawnTiles()
    {
        TileMap = new Tile[rows, columns];
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 pos = new Vector3(i, 0.0F, j);
                (Instantiate(TilePrefab, pos, transform.rotation) as GameObject).transform.parent = transform;
                // Instantiate(TilePrefab, pos, transform.rotation);
            }
        }


        //populate map
        foreach (Transform a in transform)
        {
            int i = (int)a.position.x;
            int j = (int)a.position.z;
            TileMap[i, j] = a.transform.GetComponent<Tile>();
			TileMap[i, j].type = Tile.TileType.unassigned;
        
		}

		CreateIntMap();
		AddWalls ();
    }

	#region integar cell map

	/// <summary>
	/// Creates an integer map that is used to store and define the type of each tile. 
	/// </summary>
	void CreateIntMap() {
		map = new int[rows,columns];
		tmpMap = new int[rows,columns];
 
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < rows; j++) {
				int rand = Random.Range(1,5);
				map[i,j] = rand;
				tmpMap[i,j] = map[i,j];
			}
		}
	}


	/// <summary>
	/// Get a list of tiles adjacent to the one tile the method is called with. This is done on the int map level instead of being tile based. 
	/// </summary>
	List<int> GetIntAdjacent(int x, int y)
	{
		List<int> tmp = new List<int> ();

 		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++)
				try
				{
					if (i == 0 && j == 0) // exclude middle point (self)
					{
					}
					else
					{
						a = x + i;
						b = y + j;
						if (a >= 0 && a < rows && b >= 0 && b < columns)
						{
							tmp.Add(map[x + i, y + j]);
						}
					}
				}
		catch (System.IndexOutOfRangeException e)
		{
			Debug.Log(e);
			Debug.Log("a: " + a + ". b: " + b);
		}

		return tmp;


	}
	// reemake the get neighbour methods to int map
	void CellularAutomata() {

		for (int x = 0; x < rows; x++) {
			for (int y = 0; y < columns; y++) {
				List<int> tmpList = GetIntAdjacent (x,y);

				var most = (from i in tmpList
				            group i by i into grp
				            orderby grp.Count() descending
				            select grp.Key).First();

				tmpMap[x,y] = most;
	
			}
		}
		for (int i = 0; i < rows; i++) {
			for(int j = 0; j < columns; j++) {
			map[i,j] = tmpMap[i,j];
			}
		}
	} 
	#endregion


	/// <summary>
	/// SetMaterials is called after the cellular automata iterations are performed. Then this method is called, which updates the material of each tile according to their number/type.
	/// </summary>
	void SetMaterials() {
		// if (planet.cold || planet.habitable || planet.warm) 
		// set materials for ground1, ground2, ground3, liquid and walls (maybe even gas)
		Material ground1 = new Material(Resources.Load ("Materials/smoke", typeof(Material)) as Material);
		Material ground2 = new Material(Resources.Load ("Materials/smoke", typeof(Material)) as Material);
		Material ground3 = new Material(Resources.Load ("Materials/smoke", typeof(Material)) as Material);
		Material liquid  = new Material(Resources.Load ("Materials/smoke", typeof(Material)) as Material);
		wallMat 		 = new Material(Resources.Load ("Materials/smoke", typeof(Material)) as Material);

		switch (gsh.GetCurrentPlanetType()) {
			case GameStateHandler.PlanetType.Cold:
				ground1 =	new Material(Resources.Load ("Materials/TileMats/cold_ground1", typeof(Material)) as Material);
				ground2 =	new Material(Resources.Load ("Materials/TileMats/cold_ground2", typeof(Material)) as Material);
				ground3 =	new Material(Resources.Load ("Materials/TileMats/cold_ground3", typeof(Material)) as Material);
				liquid =  	new Material(Resources.Load ("Materials/TileMats/cold_liquid", typeof(Material)) as Material);
				wallMat = 	new Material(Resources.Load ("Materials/TileMats/cold_wall", typeof(Material)) as Material);
			break;
			case GameStateHandler.PlanetType.Habitable:
				ground1 =	new Material(Resources.Load ("Materials/TileMats/habit_ground1", typeof(Material)) as Material);
				ground2 =	new Material(Resources.Load ("Materials/TileMats/habit_ground2", typeof(Material)) as Material);
				ground3 =	new Material(Resources.Load ("Materials/TileMats/habit_ground3", typeof(Material)) as Material);
				liquid =  	new Material(Resources.Load ("Materials/TileMats/habit_liquid", typeof(Material)) as Material);
				wallMat = 	new Material(Resources.Load ("Materials/TileMats/habit_wall", typeof(Material)) as Material);
			break;
			case GameStateHandler.PlanetType.Warm:
				ground1 =	new Material(Resources.Load ("Materials/TileMats/warm_ground1", typeof(Material)) as Material);
				ground2 =	new Material(Resources.Load ("Materials/TileMats/warm_ground2", typeof(Material)) as Material);
				ground3 =	new Material(Resources.Load ("Materials/TileMats/warm_ground3", typeof(Material)) as Material);
				liquid =  	new Material(Resources.Load ("Materials/TileMats/warm_liquid", typeof(Material)) as Material);
				wallMat = 	new Material(Resources.Load ("Materials/TileMats/warm_wall", typeof(Material)) as Material);
			break;

		
		}



		// Here we loop through all tiles their type, layer and material according to the integer map[x,y]
		// 1 = ground1
		// 2 = ground2 
		// 3 = ground3
		// 4 = liquid
		// 5 = gas
		// 6 = crystal
		// 7 = structure
		// 8 = spaceship

		for(int x = 0; x < rows; x++) {
			for(int y = 0; y < columns; y++) {
				if(map[x,y] == 1) {
					TileMap[x,y].type = Tile.TileType.ground1;
					TileMap[x,y].gameObject.layer = 9; // Grass
					TileMap[x,y].SetTileMat(ground1);
				} else if (map[x,y] == 2) {
					TileMap[x,y].type = Tile.TileType.ground2;
					TileMap[x,y].gameObject.layer = 10; // Sand
					TileMap[x,y].SetTileMat(ground2);
				} else if (map[x,y] == 3) {
					TileMap[x,y].type = Tile.TileType.ground3;
					TileMap[x,y].gameObject.layer = 11; // Mud
					TileMap[x,y].SetTileMat(ground3);
				} else if (map[x,y] == 4){
					TileMap[x,y].type = Tile.TileType.liquid;
					TileMap[x,y].gameObject.layer = 8; // Unwalkable
					TileMap[x,y].SetTileMat(liquid);
					TileMap[x,y].blocked = true;
				} else if (map[x,y] == 8) {

					TileMap[x,y].type = Tile.TileType.ground1;
					TileMap[x,y].gameObject.layer = 8; // Unwalkable
					TileMap[x,y].SetTileMat(ground1);
					//TileMap[x,y].blocked = true;
				}

			// not implemented yet
	
		   
				   
			}
			// if (planet.cold || planet.habitable || planet.warm) 
			// set materials for ground1, ground2, ground3, liquid and walls (maybe even gas)
		}
 	}

	/// <summary>
	/// This method spawns gas and crystals on tiles randomly throughout the planet. 
	/// </summary>
	private void AddResources() {
		for (int x = 0; x < rows; x++) {
			for (int y = 0; y < columns; y++) {
				if(!TileMap[x,y].blocked) {
					string str = "";
					int rand = Random.Range (1,1025);
					if(rand == 1) {
						if(gsh.GetCurrentPlanetType() == GameStateHandler.PlanetType.Cold) {
							str = (Random.Range (1,101) > 50) ? "GasTileBlue" : "CrystalBlue";
						} else if (gsh.GetCurrentPlanetType() == GameStateHandler.PlanetType.Habitable) {
							str = (Random.Range (1,101) > 50) ? "GasTileGreen" : "CrystalGreen";
						} else if (gsh.GetCurrentPlanetType() == GameStateHandler.PlanetType.Warm) {
							str = (Random.Range (1,101) > 50) ? "GasTileRed" : "CrystalRed";
						}
						Instantiate(Resources.Load (str),TileMap[x,y].transform.position,Quaternion.identity);
						if(str.Contains("GasTile")) {
							map[x,y] = 5;
						} else if (str.Contains ("Crystal")){
							map[x,y] = 6;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// This methods finds the outlining tiles of a blocked area (checks if a blocked tile has a non-blocked neighbour tile) and adjust the box collider on the y axis so it becomes unwalkable for the player. 
	/// </summary>
	private void SetColliderOnBlockedTiles (){
		for(int x = 0; x < rows; x++) {
			for(int y = 0; y < columns; y++) {
				if(TileMap[x,y].blocked && isNeighbourWalkable(x,y) && map[x,y] != 8) {
					TileMap[x,y].GetComponent<BoxCollider>().size = new Vector3(1.0f, 15.0f, 1.0f);

				}
			}
		}
		tileMapComplete = true;
	}

	public void SpawnSpaceship () {

		int xPos = rows / 2;
		int yPos = columns / 2;
		for (int i = xPos-2; i < xPos+2; i++) {
			for (int j = yPos-3; j < yPos+3; j++) {
				map[i,j] = 8;
				tmpMap[i,j] = 8;
			}
		}

		Instantiate (SpaceshipPrefab, new Vector3 (xPos+0.5f, 1.1F, yPos+2.5f), Quaternion.identity);


		//SpawnPlayer ();
		// player should be spawned at 30x, 33z - ALWAYS!

	}
	private void SpawnPlayer() {
		GameObject player = (Instantiate (PlayerPrefab, new Vector3 (30, 1.0f, 33), Quaternion.identity) as GameObject);
		//Camera cam = Camera.main;
		GameObject cam = GameObject.FindGameObjectWithTag ("MainCamera");
		CamSmoothFollowCustom csfc = cam.GetComponent<CamSmoothFollowCustom> ();
		csfc.target = player.transform;
	}



	/// <summary>
	/// Helper methods used by the method that adds colliders to the outer tiles of a blocked blop. 
	/// </summary>
	public bool isNeighbourWalkable(int x, int y) {
//		List<Tile> tmpList = new List<Tile> ();
		try {
			if(!TileMap[x-1,y].blocked ||  !TileMap[x,y-1].blocked || !TileMap[x+1,y].blocked || !TileMap[x,y+1].blocked )
			{
				return true;
			} else {
				return false;
			}
		} catch(System.IndexOutOfRangeException) {
			return false;
		}

	}
	#region helpers & software rot

	/// <summary>
	/// This method adds the walls around the planet level.
	/// </summary>
 	private void AddWalls() {


		if (gsh.GetCurrentPlanetType () == GameStateHandler.PlanetType.Warm) {
			WallPrefab.transform.GetComponent<Renderer>().material = Resources.Load("Materials/TileMats/warm_wall", typeof(Material)) as Material;
		} else if (gsh.GetCurrentPlanetType () == GameStateHandler.PlanetType.Habitable) {
			WallPrefab.transform.GetComponent<Renderer>().material = Resources.Load("Materials/TileMats/habit_wall", typeof(Material)) as Material;
		} else if (gsh.GetCurrentPlanetType () == GameStateHandler.PlanetType.Cold) {
			WallPrefab.transform.GetComponent<Renderer>().material = Resources.Load("Materials/TileMats/cold_wall", typeof(Material)) as Material;
		}
		//WallPrefab.transform.GetComponent<Renderer>().material = Resources.Load("Materials/PlanetMats/moon", typeof(Material)) as Material;
		WallPrefab.transform.GetComponent<Tile> ().blocked = true;
		
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {

				try{ GetTile(i+1,j); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i+1, 0.5F, j), Quaternion.identity) as GameObject).transform.parent = transform;
				}
				try{ GetTile(i-1,j); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i-1, 0.5F, j), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i,j+1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i, 0.5F, j+1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i,j-1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i, 0.5F, j-1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i+1,j+1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i+1, 0.5F, j+1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i+1,j-1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i+1, 0.5F, j-1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i-1,j+1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i-1, 0.5F, j+1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
				try{ GetTile(i-1,j-1); } catch(System.IndexOutOfRangeException) {
					(Instantiate(WallPrefab, new Vector3(i-1, 0.5F, j-1), Quaternion.identity)as GameObject).transform.parent = transform;
				}
			}
		}


	}	

	/// <summary>
	/// Help method for Getting a Tile. Returns a given tile specified by x and y coordinate. 
	/// </summary>
    public Tile GetTile(int x, int y)
    {
        return TileMap[x, y];
    }

	/// <summary>
	/// Help method for getting a list og adjacent tiles. 
	/// </summary>
    public List<Tile> GetAdjacent(Tile tile)
    {
        return GetAdjacent((int)tile.transform.position.x, (int)tile.transform.position.z);
    }

	/// <summary>
	/// Method overload for the GetAdjacent method above. This overload can be called with a x and y coordinate instead of the tile reference. 
	/// </summary>
    public List<Tile> GetAdjacent(int x, int y)
    {
        List<Tile> tmpList = new List<Tile>();
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                try
                {
                    if (i == 0 && j == 0) // exclude middle point (self)
                    {
                    }
                    else
                    {
                        a = x + i;
                        b = y + j;
                        if (a >= 0 && a < rows && b >= 0 && b < columns)
                        {
                            tmpList.Add(TileMap[x + i, y + j]);
                        }
                    }
                }
                catch (System.IndexOutOfRangeException e)
                {
					UnityEngine.Debug.Log(e);
					UnityEngine.Debug.Log("a: " + a + ". b: " + b);
                }
        //foreach(Node n in tmpList) {
        //	n.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        //}
        return tmpList;
    }

	/// <summary>
	/// This method basically does the same as the GetAdjacent method(s) above, but it does so only with the direct neighbours and not the corners of the kernel as the ones above does. 
	/// </summary>
	public List<Tile> GetNeighbours(Tile tile) {
		return GetNeighbours((int)tile.transform.position.x, (int)tile.transform.position.z);
	}
	/// <summary>
	/// This method basically does the same as the GetAdjacent method(s) above, but it does so only with the direct neighbours and not the corners of the kernel as the ones above does. 
	/// This is an overload of the GetNeighbouts method above, where you can call it with coordinates instead of a tile reference. 
	/// </summary>
	public List<Tile> GetNeighbours(int x, int y) {
		List<Tile> tmpList = new List<Tile> ();
		try {
			tmpList.Add (TileMap[x-1,y]);
			tmpList.Add (TileMap[x,y-1]);
			tmpList.Add (TileMap[x+1,y]);
			tmpList.Add (TileMap[x,y+1]);
			tmpList.Add (TileMap[x,y]);
		} catch(System.IndexOutOfRangeException) {
		}
		return tmpList;
	}


	/// <summary>
	/// Wait the specified delay.
	/// </summary>
	/// <param name="delay">Delay.</param>
	IEnumerator wait(float delay)
	{
		yield return new WaitForSeconds (delay);

	}
}
#endregion