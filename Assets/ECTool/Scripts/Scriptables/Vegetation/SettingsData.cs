using UnityEngine;

namespace ECTool.Scripts.Scriptables
{
    [System.Serializable]
    public class SettingsData : ScriptableObject
    {
        [Header("Save"), Space(5f)] 
        public string objectName = "ObjectName";
        public string objectPath = "Assets/Exports/Mesh";
    }
}