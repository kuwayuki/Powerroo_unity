using UnityEditor;
using UnityEngine;

public static class SoundManagerSetup
{
    [MenuItem("Tools/Setup SoundManager")]
    public static void Setup()
    {
        var existing = Object.FindFirstObjectByType<SoundManager>();
        if (existing != null)
        {
            Debug.Log("SoundManager already exists in the scene.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        var go = new GameObject("SoundManager");
        go.AddComponent<SoundManager>();
        EditorUtility.SetDirty(go);
        Selection.activeGameObject = go;
        Debug.Log("SoundManager created. Clips will be auto-loaded from Resources at runtime.");
    }
}
