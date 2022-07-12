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

    private State currentState;
    private ContextSteering steering;

    private void Awake()
    {
        steering = GetComponent<ContextSteering>();
    }

    private void Update()
    {
        if (currentState == State.Search && findNewPath)
        {
            PathRequestManager.Instance.RequestPath(transform.position, target.position, OnPathFound);
            findNewPath = false;
        }

        if (currentState == State.Strafe)
        {
            steering.SetInterestMode(InterestMode.Free, null);
        }

        if (Vector3.Distance(target.position, transform.position) <= strafeDistance)
        {
            StopCoroutine("FollowPath");
            findNewPath = false;
            ChangeState(State.Strafe);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");

            steering.SetInterestMode(InterestMode.Target, pathTarget);
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private IEnumerator Strafe()
    {
        while (currentState == State.Strafe)
        {
            if (Vector3.Distance(transform.position, pathTarget.position) <= pathTargetRange)
            {

            }
        }

        yield break;
    }

    private IEnumerator FollowPath()
    {
        targetIndex = 0;

        while (targetIndex < path.Length)
        {
            pathTarget.position = path[targetIndex];

            if (Vector3.Distance(transform.position, pathTarget.position) <= pathTargetRange)
            {
                targetIndex++;
            }

            yield return new WaitForEndOfFrame();
        }

        findNewPath = true;
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

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pathTarget.position, pathTargetRange);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(target.position, strafeDistance);
    }

    private enum State
    {
        Search,
        Strafe
    }
}
