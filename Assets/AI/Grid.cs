using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public struct GridSize{

	public int x;
	public int y;

	public GridSize(int x, int y){
		this.x = x;
		this.y = y;
	}

}
public class Grid : MonoBehaviour {

	public Vector2 gridWorldSize;
	public float nodeRadius;
	public LayerMask unwalkableMask;
	public TerrainType[] walkableRegions;
	LayerMask walkableMask;
	Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();


	Node[,] grid;
	float nodeDiameter;
	GridSize gridSize;
	public bool displayGridGizmos; 


	public int maxSize{

		get{
			return gridSize.x * gridSize.y;
		}
	}

	void Awake(){

		nodeDiameter = nodeRadius*2;
		gridSize.x = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSize.y = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

		foreach (TerrainType region in walkableRegions){
			walkableMask.value += region.terrainMask.value;
		//	print ((int)Mathf.Log (region.terrainMask.value, 2)+ ": "+ region.terrainWeight);
			walkableRegionsDictionary.Add((int)Mathf.Log (region.terrainMask.value, 2), region.terrainWeight);
		}
	}

	void Start(){
		StartCoroutine(WaitOneFrame());
		//CreateGrid();

	}



	IEnumerator WaitOneFrame(){

		yield return null;

		CreateGrid();

	}
	public Node NodeFromWorldPoint(Vector3 worldPosition){

		Vector2 percent = new Vector2(((worldPosition.x - transform.position.x) + gridWorldSize.x/2) / gridWorldSize.x, 
		                              (( worldPosition.z - transform.position.z) + gridWorldSize.y/2) / gridWorldSize.y);
		percent.x = Mathf.Clamp01(percent.x);
		percent.y = Mathf.Clamp01(percent.y);

		int x = Mathf.RoundToInt((gridSize.x - 1) * percent.x);
		int y = Mathf.RoundToInt((gridSize.y - 1) * percent.y);

		return grid[x,y];

	}

	public List<Node> GetNeighbours(Node node){

		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++){
			for (int y = -1; y <= 1; y++){
				if (x == 0 && y == 0){
					continue;
				}

				int checkX = node.gridSize.x + x; 
				int checkY = node.gridSize.y + y;

				if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y){
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;

	}

	void CreateGrid(){
		print (gridSize.x + " "+ gridSize.y);
		grid = new Node[gridSize.x, gridSize.y];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSize.x; x++){
			for (int y = 0; y < gridSize.y; y++){
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				int weight = 0;

				// raycast code

				if (walkable){
					Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 100, walkableMask)){
						walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out weight);
						//print (hit.collider.gameObject.layer + ": " + weight);
					}
				}

				grid[x,y] = new Node(walkable, worldPoint, new GridSize(x,y), weight);

			}
		}
	}

	void OnDrawGizmos(){

		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));

		if (grid != null && displayGridGizmos){
			foreach (Node n in grid){
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}

	[System.Serializable]
	public class TerrainType{
		public LayerMask terrainMask;
		public int terrainWeight;

	}
}
