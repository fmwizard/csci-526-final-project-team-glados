// Place this in Editor folder (e.g., Assets/Editor/ForceReserialize.cs)
using UnityEditor;

public class ForceReserialize
{
    [MenuItem("Tools/Force Reserialize Scene")]
    static void Reserialize()
    {
        AssetDatabase.ForceReserializeAssets(new[] { "Assets/Scenes/allyTutorial.unity" });
        UnityEngine.Debug.Log("Reserialization complete.");
    }
}