using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MoverComponent : IComponentData
{
    public float speed;
    public float3 direction;
}
