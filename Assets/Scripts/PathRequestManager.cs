using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : Singleton<PathRequestManager>
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private Pathfinding pathfinding;
    private bool isProcessingPath;

    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        Debug.Log("Request size: " + pathRequestQueue.Count);
    }

    /// <summary>
    /// Requests a path from the PathRequestManager to be processed when resources are available.
    /// </summary>
    /// <param name="pathStart">The start of the path</param>
    /// <param name="pathEnd">The end of the path</param>
    /// <param name="callback">The callback function for when the request is processed</param>
    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);

        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }   

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        Instance.TryProcessNext();
    }

    /// <summary>
    /// Check if the Manager is processing a request, and if not process the next request.
    /// </summary>
    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            isProcessingPath = true;

            currentPathRequest = pathRequestQueue.Dequeue();
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    /// <summary>
    /// Path request data.
    /// </summary>
    private struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        /// <summary>
        /// Creates a new path request with the given data.
        /// </summary>
        /// <param name="start">The start of the path</param>
        /// <param name="end">The end of the path</param>
        /// <param name="func">The callback function for when the request is processed</param>
        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> func)
        {
            pathStart = start;
            pathEnd = end;
            callback = func;
        }
    }
}
