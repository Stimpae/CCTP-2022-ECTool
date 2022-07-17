using UnityEngine;

namespace ECTool.Scripts.Scriptables
{
    [System.Serializable]
    public class SettingsData : ScriptableObject
    {
        [Header("Save"), Space(5f)] 
        public string objectName = "Object Name";
        public string prefabPath = "Assets/ECTool/Exports/Prefabs";
        public string proceduralObjectPath = "Assets/ECTool/Exports/ProceduralMeshObjects";
    }
}