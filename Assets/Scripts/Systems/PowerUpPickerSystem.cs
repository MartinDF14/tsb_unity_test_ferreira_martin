using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
public class PowerUpPickerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = bufferSystem.CreateCommandBuffer().AsParallelWriter();
        var random = UnityEngine.Random.Range(0, (int)PowerUps.Count);

        Entities
            .WithAll<PowerUpTag, DestroyableTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref WeaponComponent weapon) =>
            {
                var power = PowerUps.Invul;//  (PowerUps)random;

                if (power == PowerUps.MadShot)
                    commandBuffer.AddComponent(entityInQueryIndex, entity, new MadShotPowerUpComponent { timeLeft = 3f });

                if (power == PowerUps.Invul)
                    commandBuffer.AddComponent(entityInQueryIndex, entity, new InvulPowerUpTag());

                if (power == PowerUps.WeaponImprovement)
                {
                    weapon.currentWeapon.fireDelay = math.clamp(weapon.currentWeapon.fireDelay - 0.01f, 0, 100);
                    weapon.currentWeapon.shots++;
                }
                commandBuffer.RemoveComponent<PowerUpTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }


}

public enum PowerUps
{
    MadShot,
    Invul,
    WeaponImprovement,
    Count
}