using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using System.Linq;
using Unity.Collections;

public class RespawnPlayerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();

        var entities = EntityManager.GetAllEntities(Allocator.Temp);

        Entities
            .WithAll<RespawnTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, ref Translation translation, ref PlayerMovementComponent movement, ref PhysicsVelocity velocity) =>
            {
                commandBuffer.RemoveComponent<RespawnTag>(entityInQueryIndex, entity);

                commandBuffer.SetComponent(entityInQueryIndex, entity, new Translation
                {
                    Value = float3.zero
                });
                commandBuffer.SetComponent(entityInQueryIndex, entity, new PlayerMovementComponent
                {
                    accelerating = false,
                    lastDirection = float3.zero,
                    turnDirection = movement.turnDirection,
                    turnSpeed = movement.turnSpeed
                });
                commandBuffer.SetComponent(entityInQueryIndex, entity, new PhysicsVelocity
                {
                    Linear = float3.zero
                });

                commandBuffer.AddComponent(entityInQueryIndex, entity, new LifeLostTag());
            }).ScheduleParallel();
        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
