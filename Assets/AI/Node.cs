 using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {


	public bool walkable;
	public Vector3 worldPosition;
	public int gCost;
	public int hCost;
	public int fCost{
		get {
			return gCost + hCost;
		}
	}
	public GridSize gridSize;
	public int weight;
	public Node parent;
	int heapIndex;

	public Node (bool walkable, Vector3 worldPosition, GridSize gridSize, int weight){

		this.walkable = walkable;
		this.worldPosition = worldPosition;
		this.gridSize = gridSize;
		this.weight = weight;
	}

	public int HeapIndex{

		get{
			return heapIndex;
		}
		set{
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare){

		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0){
			compare = hCost.CompareTo (nodeToCompare.hCost);
		}
		return -compare;

	}
}
