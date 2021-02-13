using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

//[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class OnCollisionSystem : JobComponentSystem
{
    BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld stepPhysicsWorld;
    EndSimulationEntityCommandBufferSystem bufferSystem;

    struct OnDamageSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DestroyableTag> allLiveEntities;
        [ReadOnly] public ComponentDataFromEntity<RockComponent> allRocks;
        [ReadOnly] public ComponentDataFromEntity<DamagerTag> allBullets;
        [ReadOnly] public ComponentDataFromEntity<PlayerMovementComponent> player;
        [ReadOnly] public ComponentDataFromEntity<EnemyShootTag> enemies;
        [ReadOnly] public ComponentDataFromEntity<PowerUpTag> powerUps;
        [ReadOnly] public ComponentDataFromEntity<MadShotPowerUpComponent> madshots;
        [ReadOnly] public ComponentDataFromEntity<InvulTag> invuls;
        [ReadOnly] public ComponentDataFromEntity<FriendlyFireTag> friendlyFires;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //bullets destroys vs all - ignore powerups & other bullets
            if (allBullets.HasComponent(entityA) && !allBullets.HasComponent(entityB) && !powerUps.HasComponent(entityA))
                entityCommandBuffer.DestroyEntity(entityA);
            if (allBullets.HasComponent(entityB) && !allBullets.HasComponent(entityA) && !powerUps.HasComponent(entityB))
                entityCommandBuffer.DestroyEntity(entityB);

            //bullets destroys vs all - ignore powerups & other rocks
            if (allRocks.HasComponent(entityA) && !allRocks.HasComponent(entityB) && !powerUps.HasComponent(entityB))
            {
                entityCommandBuffer.AddComponent(entityA, new ReplicateTag());
                entityCommandBuffer.AddComponent(entityA, new ScoringComponent { score = 1 });
            }
            if (allRocks.HasComponent(entityB) && !allRocks.HasComponent(entityA) && !powerUps.HasComponent(entityA))
            {
                entityCommandBuffer.AddComponent(entityB, new ReplicateTag());
                entityCommandBuffer.AddComponent(entityB, new ScoringComponent { score = 1 });
            }


            //enemy ufos destroys vs bullets and invulshields only
            if (enemies.HasComponent(entityB) && (allBullets.HasComponent(entityA) || invuls.HasComponent(entityA)))
            {
                entityCommandBuffer.AddComponent(entityB, new EnemyDestroyedTag());
                entityCommandBuffer.AddComponent(entityB, new ScoringComponent { score = 10 });
            }
            if (enemies.HasComponent(entityA) && (allBullets.HasComponent(entityB) || invuls.HasComponent(entityB)))
            {
                entityCommandBuffer.AddComponent(entityA, new EnemyDestroyedTag());
                entityCommandBuffer.AddComponent(entityA, new ScoringComponent { score = 10 });
            }


            //player destroys vs rocks
            if (player.HasComponent(entityA) && allRocks.HasComponent(entityB))
                entityCommandBuffer.AddComponent(entityA, new RespawnTag());
            if (player.HasComponent(entityB) && allRocks.HasComponent(entityA))
                entityCommandBuffer.AddComponent(entityB, new RespawnTag());

            //player destroys vs bullets - ignore madshots
            if (player.HasComponent(entityA) && allBullets.HasComponent(entityB) && !madshots.HasComponent(entityB) && !friendlyFires.HasComponent(entityB))
                entityCommandBuffer.AddComponent(entityA, new RespawnTag());
            if (player.HasComponent(entityB) && allBullets.HasComponent(entityA) && !madshots.HasComponent(entityA) && !friendlyFires.HasComponent(entityA))
                entityCommandBuffer.AddComponent(entityB, new RespawnTag());

            //player picks powerups
            if (player.HasComponent(entityA) && powerUps.HasComponent(entityB))
            {
                entityCommandBuffer.AddComponent(entityA, new PowerUpTag());
                entityCommandBuffer.DestroyEntity(entityB);
            }
            if (player.HasComponent(entityB) && powerUps.HasComponent(entityA))
            {
                entityCommandBuffer.AddComponent(entityB, new PowerUpTag());
                entityCommandBuffer.DestroyEntity(entityA);
            }
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new OnDamageSystemJob() { entityCommandBuffer = bufferSystem.CreateCommandBuffer() };
        job.allLiveEntities = GetComponentDataFromEntity<DestroyableTag>(true);
        job.allRocks = GetComponentDataFromEntity<RockComponent>(true);
        job.allBullets = GetComponentDataFromEntity<DamagerTag>(true);
        job.player = GetComponentDataFromEntity<PlayerMovementComponent>(true);
        job.enemies = GetComponentDataFromEntity<EnemyShootTag>(true);
        job.powerUps = GetComponentDataFromEntity<PowerUpTag>(true);
        job.madshots = GetComponentDataFromEntity<MadShotPowerUpComponent>(true);
        job.invuls = GetComponentDataFromEntity<InvulTag>(true);
        job.friendlyFires = GetComponentDataFromEntity<FriendlyFireTag>(true);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        bufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
