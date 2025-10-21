using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnData
{
    public GameObject prefab;

    public Difficulty difficulty;
    public Vector3 position;
}

[CreateAssetMenu(fileName = "LevelConfig", menuName = "DDA/Configs")]
public class LevelConfig : ScriptableObject {
    public List<SpawnData> data = new List<SpawnData>();
}