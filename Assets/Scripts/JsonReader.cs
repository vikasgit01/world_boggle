using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script is placed on Canvas gameObject
public class JsonReader : MonoBehaviour
{
    //static object of leveldata
    public static LevelData m_lD = new LevelData();

    //textasset of actual level data
    [SerializeField] private TextAsset m_levelData;

    void Start()
    {
        m_lD = JsonUtility.FromJson<LevelData>(m_levelData.text);
    }
}

[System.Serializable]
public class LevelData
{
    public Data[] data;
}

[System.Serializable]
public class Data
{
    public int bugCount;
    public int wordCount;
    public float timeSec;
    public int totalScore;
    public GridSize gridSize;
    public int levelType;
    public GridData[] gridData;
}

[System.Serializable]
public class GridSize
{
    public int x;
    public int y;
}

[System.Serializable]
public struct GridData
{
    public int tileType;
    public string letter;
}
