using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SelfDestroySystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
           .ForEach((Entity entity, int entityInQueryIndex, ref SelfDestroyComponent destroyComponent) =>
           {
               destroyComponent.currentLifeTime += deltaTime;
               if (destroyComponent.currentLifeTime >= destroyComponent.lifeSpan)
               {
                   commandBuffer.DestroyEntity(entityInQueryIndex, entity);
               }
           }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
