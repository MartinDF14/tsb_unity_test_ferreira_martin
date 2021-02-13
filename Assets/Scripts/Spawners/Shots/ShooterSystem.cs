using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShootingSystem : SystemBase
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
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref WeaponComponent weapon, ref Rotation rotation, ref Translation translation, in ShooterComponent spawner) =>
            {
                if (weapon.shooting)
                {
                    if (weapon.currentWeapon.realoadingTime <= 0)
                    {
                        weapon.currentWeapon.realoadingTime = weapon.currentWeapon.fireDelay;
                        bool top = true;
                        for (int i = 0; i < weapon.currentWeapon.shots; i++)
                        {
                            var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);

                            float3 fwd = math.forward(rotation.Value);
                            commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
                            {
                                Value = translation.Value + fwd
                            });

                            if (i != 0)
                                fwd = Quaternion.AngleAxis((top ? i : -i) * 3, math.forward()) * fwd;
                            commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                            {
                                speed = weapon.currentWeapon.bulletSpeed,
                                direction = fwd
                            });
                            top = !top;
                            commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                        }
                    }
                    else if (weapon.currentWeapon.realoadingTime > 0)
                    {
                        weapon.currentWeapon.realoadingTime -= deltaTime;
                    }
                }
            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
