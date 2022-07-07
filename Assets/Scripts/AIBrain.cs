using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public Transform interestPoint;

    private Pathfinding pathFinding;

    private void Awake()
    {
        pathFinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        pathFinding.FindPath(transform.position, interestPoint.position);
    }
}
