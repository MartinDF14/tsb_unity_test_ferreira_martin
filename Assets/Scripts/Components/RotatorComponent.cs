using Unity.Entities;

[GenerateAuthoringComponent]
public struct RotatorComponent : IComponentData
{
    public float speed;
    public float direction;
}
