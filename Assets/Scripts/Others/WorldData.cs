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
    public int MaxSpeed = 15;
    public int MAX_SIZE = 5;
    public int ENEMIES_ALIVE = 3;

    [HideInInspector]
    public int Lives = MAX_LIVES;
    public int Level = 0;
    public int Score = 0;


    private void Awake()
    {
        _instance = this;
    }

    public void AddScore(int value)
    {
        Score += value;
    }
}
