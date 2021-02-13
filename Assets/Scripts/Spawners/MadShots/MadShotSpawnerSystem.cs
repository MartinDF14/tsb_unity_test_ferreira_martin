using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MadShotSpawnerSystem : SystemBase
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
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("MadShotSpawnerSystem")
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref WeaponComponent weapon, ref MadShotPowerUpComponent madshot, ref Rotation rotation, ref Translation translation, in MadShotSpawnerComponent spawner) =>
            {
                madshot.timeLeft -= deltaTime;
                if (madshot.timeLeft > 0)
                {
                    if (weapon.shooting)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);

                            float3 fwd = math.forward(rotation.Value) * i;
                            commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                            {
                                Value = translation.Value + fwd * 2
                            });

                            commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                            {
                                speed = 5,
                                direction = fwd
                            });
                            commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                            commandBuffer.AddComponent(entityInQueryIndex, instance, new MadShotPowerUpComponent());
                        }
                    }
                }
                else
                    commandBuffer.RemoveComponent<MadShotPowerUpComponent>(entityInQueryIndex, entity);

            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
