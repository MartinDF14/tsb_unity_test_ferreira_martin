using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class ShotSpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new ShotSpawnerComponent
        {
            prefab = conversionSystem.GetPrimaryEntity(prefab)
        };

        dstManager.AddComponentData(entity, spawnerData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }
}
