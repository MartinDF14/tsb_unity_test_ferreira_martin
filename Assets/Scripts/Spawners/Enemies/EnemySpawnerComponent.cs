using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity smallRock;
    public Entity medRock;
    public Entity bigRock;
    public Entity enemyShipA;
}
