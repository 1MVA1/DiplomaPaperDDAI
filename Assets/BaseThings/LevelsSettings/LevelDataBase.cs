using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{
    public string sceneName;

    public List<LevelConfig> enemyConfigs;
    public List<LevelConfig> platformingConfigs;
}

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "DDA/Database")]
public class LevelDatabase : ScriptableObject {
    public List<LevelData> levels = new List<LevelData>();
}
