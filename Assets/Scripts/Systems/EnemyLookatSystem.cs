using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using System.Linq;

public class EnemyLookatSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entities = EntityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
        Entity player = entities.Where(x => EntityManager.HasComponent<PlayerMovementComponent>(x)).First();
        Translation playerPos = EntityManager.GetComponentData<Translation>(player);
        var time = Time.DeltaTime;

        Entities
            .WithAll<EnemyShootTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, ref Translation translation) =>
            {
                var lookatdir = playerPos.Value - translation.Value;
                quaternion rot = quaternion.LookRotationSafe(lookatdir, math.up());
                rotation.Value = math.slerp(rotation.Value, rot, 0.1f);
            }).Run();

    }
}
