using Unity.Collections;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    static WorldData _instance;
    public static WorldData Instance
    {
        get { return _instance; }
    }

    [ReadOnly] public const int WORLD_WIDTH = 16;
    [ReadOnly] public const int WORLD_HEIGHT = 9;
    [ReadOnly] public const int ENEMIES_PREFABS = 3;
    [ReadOnly] public const int MAX_LIVES = 3;
    [HideInInspector]
    public int ENEMIES_ALIVE = 3;
    [HideInInspector]
    public int Lives = MAX_LIVES;
    [HideInInspector]
    public int Score = 0;

    public int MaxSpeed = 17;
    public int Level = 0;


    private void Awake()
    {
        _instance = this;
    }

    public void AddScore(int value)
    {
        Score += value;
    }
}
