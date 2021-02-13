using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float maxSpeed = WorldData.Instance.MaxSpeed;

        Entities
            .ForEach((ref Translation translation, ref PlayerMovementComponent movementComponent, ref PhysicsVelocity velocity, ref Rotation rotation) =>
            {
                if (movementComponent.accelerating)
                {
                    movementComponent.lastDirection = math.mul(rotation.Value, new float3(0f, 0f, 1f));
                    velocity.Linear += 10 * movementComponent.lastDirection * deltaTime;
                    velocity.Linear.x = math.clamp(velocity.Linear.x, -maxSpeed, maxSpeed);
                    velocity.Linear.y = math.clamp(velocity.Linear.y, -maxSpeed, maxSpeed);
                    velocity.Linear.z = 0;
                }
                else
                {
                    var magnitude = math.sqrt(math.pow(velocity.Linear.x, 2) + math.pow(velocity.Linear.y, 2));
                    if (magnitude > 1f)
                        velocity.Linear -= velocity.Linear * deltaTime * 0.5f;
                    else
                        velocity.Linear = float3.zero;
                }

            }).Run();
    }
}
