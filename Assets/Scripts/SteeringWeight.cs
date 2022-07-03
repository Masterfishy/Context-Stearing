using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Steering Weights/")]
public class SteeringWeight : ScriptableObject
{
    public float weight;

    public static implicit operator float(SteeringWeight reference)
    {
        return reference.weight;
    }
}
