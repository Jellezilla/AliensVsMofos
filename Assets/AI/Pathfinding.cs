using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;


public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	// Use this for initialization
	void Awake () {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos){
		StartCoroutine(FindPath (startPos, targetPos));
	}
	 
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		if (startNode.walkable && targetNode.walkable){
			Heap<Node> openSet = new Heap<Node>(grid.maxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0){
				
				Node currentNode = openSet.RemoveFirst() ;
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode){
					sw.Stop ();
					pathSuccess = true;
					//print ("Path found: "+sw.ElapsedMilliseconds+ " ms");
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)){
					if (!neighbour.walkable || closedSet.Contains(neighbour)){
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.weight;
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour)){
							openSet.Add(neighbour);
						}
						else {
							openSet.UpdateItem(neighbour);
						}
					}
				}
			}
		}
	
		yield return null;
		if (pathSuccess){
			waypoints = RetracePath(startNode, targetNode);
		}
		requestManager.FinishedProccessingPath(waypoints, pathSuccess);
	}

	Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode){
			path.Add(currentNode);
			currentNode = currentNode.parent;
		} 
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;

	}

	Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector3.zero;
		for (int i = 1; i < path.Count; i++){
			Vector2 directionNew = new Vector2(path[i-1].gridSize.x - path[i].gridSize.x, path[i-1].gridSize.y - path[i].gridSize.y);
			if (directionOld != directionNew){
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node a, Node b){

		int distX = Mathf.Abs (a.gridSize.x - b.gridSize.x);
		int distY = Mathf.Abs (a.gridSize.y - b.gridSize.y);

		if (distX > distY){
			return 14*distY + 10 * (distX - distY);
		}

		return 14 * distX + 10 * (distY - distX);

	}
}
