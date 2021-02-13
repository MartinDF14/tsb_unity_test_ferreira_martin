using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class ReplicateSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();
        var random = UnityEngine.Random.Range(0, Mathf.Infinity);
        var rnd = Unity.Mathematics.Random.CreateFromIndex((uint)random);

        Entities
            .WithAll<ReplicateTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref MoverComponent mover, ref EnemySpawnerComponent spawner, ref RockComponent rock, ref Translation translation, ref Rotation rotation) =>
            {
                if (rock.size > 1)
                {
                    commandBuffer.RemoveComponent<ReplicateTag>(entityInQueryIndex, entity);

                    for (int i = 0; i < 2; i++)
                    {
                        var rockSize = rock.size - 1;
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, rockSize == 1 ? spawner.smallRock : rockSize == 2 ? spawner.medRock : spawner.bigRock);

                        commandBuffer.SetComponent(entityInQueryIndex, instance, new RockComponent { size = rockSize });

                        float3 direction = math.mul(rotation.Value, new float3(0f, 1f, 0f));
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = translation.Value + direction * rnd.NextFloat3(-1f, 1f) });

                        commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                        {
                            speed = mover.speed * 0.5f,
                            direction = direction
                        });
                        commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                    }

                }

                commandBuffer.DestroyEntity(entityInQueryIndex, entity);

            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}