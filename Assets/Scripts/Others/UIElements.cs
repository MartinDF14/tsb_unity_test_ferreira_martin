using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public Image[] lives;
    public Text score;
    public Text level;

    static UIElements _instance;
    public static UIElements Instance
    {
        get { return _instance; }
    }

    int _lives;
    int _score;
    int _level;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (_lives != WorldData.Instance.Lives)
            SetLives(WorldData.Instance.Lives);

        if (_level != WorldData.Instance.Level)
            SetLevel(WorldData.Instance.Level);

        if (_score != WorldData.Instance.Score)
            SetScore(WorldData.Instance.Score);
    }

    void SetLives(int value)
    {
        _lives = value;
        for (int i = 0; i < lives.Length; i++)
            lives[i].gameObject.SetActive(i <= value - 1);
    }

    void SetLevel(int value)
    {
        _level = value;
        level.text = "" + value;
    }

    void SetScore(int value)
    {
        _score = value;
        score.text = "" + value;
    }
}
