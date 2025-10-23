using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameObjectData
{
    public GameObject prefab;

    public Difficulty difficulty;
    public Vector3 position;
}

[System.Serializable]
public class GameObjectConfig {
    public List<GameObjectData> data = new List<GameObjectData>();
}

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/Config")]
public class LevelConfig : ScriptableObject 
{
    public List<GameObjectConfig> enemyConfig = new List<GameObjectConfig>();
    public List<GameObjectConfig> platformingConfig = new List<GameObjectConfig>();
}