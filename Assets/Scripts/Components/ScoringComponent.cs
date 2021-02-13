using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ScoringComponent : IComponentData
{
    int _score;
    public int score
    {
        set
        {
            _score = value;
            WorldData.Instance.AddScore(value);
        }
    }
}
