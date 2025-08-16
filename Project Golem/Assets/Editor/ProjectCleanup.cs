using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Cinemachine;

public static class ProjectCleanup
{
    // 1) Ping all objects with Missing Scripts (scene only)
    [MenuItem("Tools/Cleanup/Ping Objects With Missing Scripts (Scene)")]
    public static void PingMissingScriptsInScene()
    {
        var count = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            var comps = go.GetComponents<Component>();
            foreach (var c in comps)
            {
                if (c == null)
                {
                    count++;
                    Debug.LogWarning($"Missing script on: {GetPath(go)}", go);
                    break;
                }
            }
        }
        Debug.Log($"Done. Objects with missing scripts in scene: {count}");
    }

    // 2) Remove all Missing Scripts from selection (works on prefabs too)
    [MenuItem("Tools/Cleanup/Remove Missing Scripts From Selection")]
    public static void RemoveMissingFromSelection()
    {
        int removed = 0;
        foreach (var obj in Selection.gameObjects)
        {
            removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            Debug.Log($"Removed {removed} missing scripts from {obj.name}", obj);
        }
        Debug.Log($"Total removed: {removed} (select objects/prefabs first).");
    }

    // 3) Ping VCams that still use the old PostProcessing extension
    [MenuItem("Tools/Cleanup/Ping VCams Using Legacy PostProcessing")]
    public static void PingLegacyCinemachinePP()
    {
#if CINEMACHINE_POST_PROCESSING_V2
        Debug.Log("You have PostProcessing v2 scripting define; this check may not apply.");
#endif
        int found = 0;
        foreach (var vcam in Object.FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None))
        {
            // Old extension type lives in the package; we detect via GetComponent with string
            var ext = vcam.GetComponent("CinemachinePostProcessing");
            if (ext != null)
            {
                found++;
                Debug.LogWarning($"VCam '{vcam.name}' uses legacy Cinemachine Post Processing. Remove it and use 'Cinemachine Volume Settings' instead.", vcam);
            }
        }
        Debug.Log($"Done. VCams with legacy PP extension: {found}");
    }

    static string GetPath(GameObject go)
    {
        var stack = new List<string>();
        var t = go.transform;
        while (t != null)
        {
            stack.Add(t.name);
            t = t.parent;
        }
        stack.Reverse();
        return string.Join("/", stack);
    }
}
