using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        int maxSpeed = WorldData.Instance.MaxSpeed;
        Entities
           .ForEach((ref InputComponent input, ref PlayerMovementComponent movement, ref Rotation rotation, ref WeaponComponent weapon) =>
           {
               bool isUp = Input.GetKey(input.upKey);
               bool isRight = Input.GetKey(input.rightKey);
               bool isLeft = Input.GetKey(input.leftKey);
               bool shooting = Input.GetKey(input.shootKey);

               weapon.shooting = shooting;
               movement.accelerating = isUp;
               movement.turnDirection = isRight ? -1 : isLeft ? 1 : 0;
           }).Run();
    }
}
