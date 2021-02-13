using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject smallRock;
    public GameObject medRock;
    public GameObject bigRock;
    public GameObject enemyshipA;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new EnemySpawnerComponent
        {
            smallRock = conversionSystem.GetPrimaryEntity(smallRock),
            medRock = conversionSystem.GetPrimaryEntity(medRock),
            bigRock = conversionSystem.GetPrimaryEntity(bigRock),
            enemyShipA = conversionSystem.GetPrimaryEntity(enemyshipA)
        };

        dstManager.AddComponentData(entity, spawnerData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(smallRock);
        referencedPrefabs.Add(medRock);
        referencedPrefabs.Add(bigRock);
        referencedPrefabs.Add(enemyshipA);
    }
}
