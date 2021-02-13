using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerMovementComponent : IComponentData
{
    public float turnDirection;
    public float turnSpeed;
    public float3 lastDirection;
    public bool accelerating;
}
