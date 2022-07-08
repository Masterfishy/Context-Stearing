using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public Transform target;
    private Vector3[] path;
    private int targetIndex;
    private bool findNewPath = true;

    private void Update()
    {
        if (findNewPath)
        {
            PathRequestManager.Instance.RequestPath(transform.position, target.position, OnPathFound);
            findNewPath = false;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = newPath;
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
        }
        findNewPath = true;
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (targetIndex < path.Length)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                currentWaypoint = path[targetIndex];
            }

            yield return new WaitForEndOfFrame();

            // Move the Enemy (set a new interest)
        } 
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(path[i], 0.1f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
