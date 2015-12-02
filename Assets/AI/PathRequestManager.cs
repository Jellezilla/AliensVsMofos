using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {


	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinding pathfinding;
	bool isProcessingPath;

	void Awake(){

		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback){
		PathRequest newRequest = new PathRequest(start, end, callback);
		instance.pathRequestQueue.Enqueue(newRequest); // add
		instance.TryProcessNext();

	}

	void TryProcessNext(){

		if (!isProcessingPath && pathRequestQueue.Count > 0){
			currentPathRequest = pathRequestQueue.Dequeue(); // get first item in queue and takes it out
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}

	}

	public void FinishedProccessingPath(Vector3[] path, bool success){
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();

	}

	struct PathRequest{

		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback){

			pathStart = _start;
			pathEnd = _end;
			callback = _callback;

		}

	}
}
