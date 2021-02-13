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
            //.WithNone<EnemyShootTag>()
            .ForEach((ref Translation translation, ref MoverComponent moverComponent) =>
            {
                translation.Value.y += moverComponent.direction.y * time * moverComponent.speed;
                translation.Value.x += moverComponent.direction.x * time * moverComponent.speed;
            }).ScheduleParallel();

        var entities = EntityManager.GetAllEntities(Allocator.Temp);
        Entity player = entities.Where(x => EntityManager.HasComponent<PlayerMovementComponent>(x)).First();
        Translation playerPos = EntityManager.GetComponentData<Translation>(player);
        Entities
            .WithAll<InvulTag>()
            .WithNone<PlayerMovementComponent>()
            .ForEach((ref Translation translation) =>
            {
                translation.Value = playerPos.Value;
            }).ScheduleParallel();
    }

}

