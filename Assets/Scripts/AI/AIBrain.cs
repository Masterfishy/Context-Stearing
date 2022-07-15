using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public Transform target;
    public Transform pathTarget;

    public float strafeDistance;
    public float pathTargetRange;

    private Vector3[] path;
    private int targetIndex;
    private bool findNewPath = true;

    private Vector3 pathTargetPosition;

    private ContextSteering steering;

    private void Awake()
    {
        steering = GetComponent<ContextSteering>();

        pathTargetPosition = transform.position;
    }

    private void Update()
    {
        if (findNewPath)
        {
            steering.SetMoveMode(MoveMode.Target);
            PathRequestManager.Instance.RequestPath(transform.position, target.position, OnPathFound);
            findNewPath = false;
        }

        if (path != null && path.Length > 0)
        {
            findNewPath = Vector3.Distance(target.position, path[path.Length - 1]) > 1f;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        if (path.Length <= 0)
        {
            findNewPath = true;
            yield break;
        }

        targetIndex = 0;
        pathTargetPosition = path[targetIndex];
        steering.MoveTo(pathTargetPosition);

        while (targetIndex < path.Length)
        {
            if (Vector3.Distance(transform.position, pathTargetPosition) < pathTargetRange)
            {
                pathTargetPosition = path[targetIndex];
                steering.MoveTo(pathTargetPosition);

                targetIndex++;
            }

            yield return new WaitForEndOfFrame();
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

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(pathTargetPosition, pathTargetRange);
        }
    }
}
