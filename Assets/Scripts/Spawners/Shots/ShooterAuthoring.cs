using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class ShooterAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject prefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new ShooterComponent
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
