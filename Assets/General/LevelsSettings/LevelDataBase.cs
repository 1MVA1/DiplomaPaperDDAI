using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LevelData
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [HideInInspector]
    public string sceneName;

    public LevelConfig config;
}

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/Database")]
public class LevelDataBase : ScriptableObject {
    public List<LevelData> levels = new List<LevelData>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelDataBase))]
public class LevelDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelDataBase db = (LevelDataBase)target;

        foreach (var level in db.levels)
        {
            if (level.sceneAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(level.sceneAsset);
                level.sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(db);
    }
}
#endif