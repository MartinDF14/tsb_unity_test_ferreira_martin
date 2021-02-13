using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Collections.LowLevel.Unsafe.UnsafeList;

public class EnemySpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var entities = EntityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
        if (entities != null && entities.Count() > 0)
        {
            var count = entities.Where(x => EntityManager.HasComponent<RockComponent>(x));
            if (count != null && count.Count() > 0)
            {
                WorldData.Instance.ENEMIES_ALIVE = count.Count();
            }
        }

        if (WorldData.Instance.ENEMIES_ALIVE > WorldData.ENEMIES_PREFABS)
            return;

        WorldData.Instance.Level++;
        WorldData.Instance.Level++;
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();

        var maxRocks = 2 + WorldData.Instance.Level;
        var level = WorldData.Instance.Level;
        var speed = WorldData.Instance.MaxSpeed;

        var random = UnityEngine.Random.Range(0, Mathf.Infinity);
        var rnd = Unity.Mathematics.Random.CreateFromIndex((uint)random);

        Entities
            .WithName("EnemySpawnerSystem")
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref EnemySpawnerComponent spawner) =>
            {
                for (int i = 0; i < maxRocks; i++)
                {
                    var rockSize = math.clamp(rnd.NextInt(1, level + 2), 1, 4);
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, rockSize == 1 ? spawner.smallRock : rockSize == 2 ? spawner.medRock : spawner.bigRock);

                    commandBuffer.SetComponent(entityInQueryIndex, instance, new RockComponent { size = rockSize });

                    var side = rnd.NextInt(0, 4);
                    var spawnPos = new float2(
                        side == 0 ? -16 : side == 1 ? 16 : rnd.NextInt(-16, 16),
                        side == 2 ? 9 : side == 3 ? 9 : rnd.NextInt(-9, 9));
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = new float3(spawnPos.x, spawnPos.y, 0) });

                    var directionCircle = new float2(rnd.NextFloat(-1f, 1f), rnd.NextFloat(-1f, 1f));
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                    {
                        speed = math.clamp(level + 5 - rockSize, -speed, speed),
                        direction = new float3(directionCircle.x, directionCircle.y, 0)
                    });

                    commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                }

                if (level % 2 == 0)
                {
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.enemyShipA);

                    var side = rnd.NextInt(0, 4);
                    var spawnPos = new float2(
                        side == 0 ? -16 : side == 1 ? 16 : rnd.NextInt(-16, 16),
                        side == 2 ? 9 : side == 3 ? 9 : rnd.NextInt(-9, 9));
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = new float3(spawnPos.x, spawnPos.y, 0) });

                    var directionCircle = new float2(rnd.NextFloat(-1f, 1f), rnd.NextFloat(-1f, 1f));
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                    {
                        speed = math.clamp(level * 2.5f, -speed, speed),
                        direction = new float3(directionCircle.x, directionCircle.y, 0)
                    });


                    commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                }
            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
