using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class InvulSpawnerSystem : SystemBase
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
            .WithName("InvulSpawnerSystem")
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref InvulPowerUpTag invul, ref InvulSpawnerComponent spawner, ref Translation translation) =>
            {
                var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                {
                    Value = translation.Value
                });
                commandBuffer.RemoveComponent<InvulPowerUpTag>(entityInQueryIndex, entity);
                commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }


}
