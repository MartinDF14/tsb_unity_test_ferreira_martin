using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        Entities
            .ForEach((ref Rotation rotation, ref RotatorComponent rotatorComponent) =>
            {
                quaternion zRot = quaternion.RotateZ(rotatorComponent.direction * time * rotatorComponent.speed);
                rotation.Value = math.mul(rotation.Value, zRot);
            }).ScheduleParallel();
    }

}

