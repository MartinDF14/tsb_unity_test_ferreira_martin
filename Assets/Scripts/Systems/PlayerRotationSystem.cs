using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
public class PlayerRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .ForEach((ref Rotation rotation, ref PlayerMovementComponent movementComponent) =>
            {
                if (movementComponent.turnDirection != 0)
                {
                    var rot = quaternion.RotateY(movementComponent.turnDirection * deltaTime * movementComponent.turnSpeed);
                    rotation.Value = math.mul(rotation.Value, rot);
                }
            }).Run();
    }
}
