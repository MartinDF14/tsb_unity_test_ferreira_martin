using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;

public class RelocatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var width = WorldData.WORLD_WIDTH;
        var height = WorldData.WORLD_HEIGHT;
        Entities
            .ForEach((ref Translation translation) =>
            {
                if (translation.Value.x > width)
                    translation.Value.x = -width;

                if (translation.Value.x < -width)
                    translation.Value.x = width;

                if (translation.Value.y > height)
                    translation.Value.y = -height;

                if (translation.Value.y < -height)
                    translation.Value.y = height;

            }).ScheduleParallel();
    }

}
