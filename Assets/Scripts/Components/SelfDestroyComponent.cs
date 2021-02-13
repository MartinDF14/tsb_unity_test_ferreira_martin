using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SelfDestroyComponent : IComponentData
{
    public float lifeSpan;
    public float currentLifeTime;
}
