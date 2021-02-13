using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class PowerUpSpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithAll<EnemyDestroyedTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref PowerUpSpawnerComponent spawner, ref Translation translation) =>
            {
                var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.powerUp);
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                {
                    Value = translation.Value
                });
                commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }


}
