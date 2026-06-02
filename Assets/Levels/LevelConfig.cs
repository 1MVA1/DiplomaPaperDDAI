
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[System.Serializable]
public class BaseObjectData
{
    public Diff difficulty = Diff.Medium;
    public Vector3 position;
}

[System.Serializable]
public class SpikeData : BaseObjectData
{
    public float size = 1f;
    public float rotation = 0f;
}
[System.Serializable]
public class SawData : BaseObjectData
{
    public float size = 1f;
    public float speed = 1f;
    public float timeStop = 0f;
    public float acceleration = 0f;
    public float maxSpeed = 5f;
    public Vector2[] points;
    public bool isCycle = true;
}
[System.Serializable]
public class VoltZoneData : BaseObjectData
{
    public float size = 1f;
    public bool isHorizontal = true;
}
[System.Serializable]
public class EnemyData : BaseObjectData
{
    public bool isMovingRight = true;
}


[System.Serializable]
public class SpikeConfig
{
    public List<SpikeData> data = new();
}
[System.Serializable]
public class SawConfig
{
    public List<SawData> data = new();
}
[System.Serializable]
public class VoltZoneConfig
{
    public List<VoltZoneData> data = new();
}
[System.Serializable]
public class EnemyConfig
{
    public List<EnemyData> dataCrab = new();
    public List<EnemyData> dataShooter = new();
    public List<EnemyData> dataStalker = new();
}

[System.Serializable]
public class Level
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [HideInInspector]
    public string sceneName;

    public List<SpikeConfig> spikeConfigs = new();
    public List<SawConfig> sawConfigs = new();
    public List<VoltZoneConfig> voltZoneConfigs = new();
    public List<EnemyConfig> enemyConfigs = new();
}

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Level/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public GameObject spike;
    public GameObject saw;
    public GameObject voltZone;

    public GameObject crab;
    public GameObject shooter;
    public GameObject stalker;

    public List<Level> levels = new();
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelsConfig))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelsConfig db = (LevelsConfig)target;

        foreach (var level in db.levels)
        {
            if (level.sceneAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(level.sceneAsset);
                level.sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(db);
        }
    }
}
#endif