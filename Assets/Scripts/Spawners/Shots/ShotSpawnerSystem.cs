using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class ShotSpawnerSystem : SystemBase
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
            .WithNone<MadShotPowerUpComponent>()
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref WeaponComponent weapon, ref Rotation rotation, ref Translation translation, ref PhysicsVelocity velocity, in ShotSpawnerComponent spawner) =>
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
                                Value = translation.Value + fwd * 2 
                            });

                            float3 angle = fwd;
                            if (i != 0)
                                angle = Quaternion.AngleAxis((top ? i : -i) * 3, math.forward()) * fwd;

                            var fwdVel = velocity.Linear * math.forward(rotation.Value);
                            var fwdVelDif = fwdVel.x + fwdVel.y;
                            commandBuffer.SetComponent(entityInQueryIndex, instance, new MoverComponent
                            {
                                speed = weapon.currentWeapon.bulletSpeed + fwdVelDif,
                                direction = angle
                            });
                            top = !top;
                            commandBuffer.AddComponent(entityInQueryIndex, instance, new DestroyOnNewWorldTag());
                            if (weapon.currentWeapon.friendlyFire)
                                commandBuffer.AddComponent(entityInQueryIndex, instance, new FriendlyFireTag());
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
