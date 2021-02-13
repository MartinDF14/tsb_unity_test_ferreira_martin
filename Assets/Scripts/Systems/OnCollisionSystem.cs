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

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            //bullets destroys vs all - ignore powerups & other bullets
            if (allBullets.HasComponent(entityA) && !allBullets.HasComponent(entityB))
                entityCommandBuffer.DestroyEntity(entityA);
            if (allBullets.HasComponent(entityB) && !allBullets.HasComponent(entityA))
                entityCommandBuffer.DestroyEntity(entityB);

            //bullets destroys vs all - ignore other rocks
            if (allRocks.HasComponent(entityA) && !allRocks.HasComponent(entityB))
            {
                entityCommandBuffer.AddComponent(entityA, new ReplicateTag());
            }
            if (allRocks.HasComponent(entityB) && !allRocks.HasComponent(entityA))
            {
                entityCommandBuffer.AddComponent(entityB, new ReplicateTag());
            }


            //enemy ufos destroys vs bullets 
            if (enemies.HasComponent(entityB) && allBullets.HasComponent(entityA))
            {
                entityCommandBuffer.AddComponent(entityB, new EnemyDestroyedTag());
            }
            if (enemies.HasComponent(entityA) && allBullets.HasComponent(entityB))
            {
                entityCommandBuffer.AddComponent(entityA, new EnemyDestroyedTag());
            }


            //player destroys vs rocks
            if (player.HasComponent(entityA) && allRocks.HasComponent(entityB))
                entityCommandBuffer.AddComponent(entityA, new RespawnTag());
            if (player.HasComponent(entityB) && allRocks.HasComponent(entityA))
                entityCommandBuffer.AddComponent(entityB, new RespawnTag());

            //player destroys vs bullets
            if (player.HasComponent(entityA) && allBullets.HasComponent(entityB))
                entityCommandBuffer.AddComponent(entityA, new RespawnTag());
            if (player.HasComponent(entityB) && allBullets.HasComponent(entityA))
                entityCommandBuffer.AddComponent(entityB, new RespawnTag());
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

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        bufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
