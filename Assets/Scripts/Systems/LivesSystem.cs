using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using System.Linq;
using UnityEngine;

public class LivesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entities = EntityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
        Entity player = entities.Where(x => EntityManager.HasComponent<PlayerMovementComponent>(x)).First();

        if (EntityManager.HasComponent<LifeLostTag>(player))
        {
            WorldData.Instance.Lives--;
            EntityManager.RemoveComponent<LifeLostTag>(player);
            if (WorldData.Instance.Lives < 0)
            {
                EntityManager.SetComponentData(player, new WeaponComponent
                {
                    currentWeapon = new Weapon
                    {
                        bulletSpeed = 15,
                        fireDelay = 0.1f,
                        realoadingTime = 0,
                        shots = 1
                    }
                });

                WorldData.Instance.Lives = WorldData.MAX_LIVES;
                WorldData.Instance.Level = 0;
                WorldData.Instance.Score = 0;

                var destroyables = entities.Where(x => EntityManager.HasComponent<DestroyOnNewWorldTag>(x));
                foreach (var item in destroyables)
                    if (EntityManager.Exists(item))
                        EntityManager.DestroyEntity(item);
            }
            else
            {
                WorldData.Instance.Lives = WorldData.Instance.Lives;
            }

        }

        entities.Dispose();
    }
}
