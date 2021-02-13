using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;

public class MoverSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        Entities
            .ForEach((ref Translation translation, ref MoverComponent moverComponent) =>
            {
                translation.Value.y += moverComponent.direction.y * time * moverComponent.speed;
                translation.Value.x += moverComponent.direction.x * time * moverComponent.speed;
            }).ScheduleParallel();
    }

}

