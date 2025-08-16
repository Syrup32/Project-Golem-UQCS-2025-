using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [Header("Attach points on the base prefab")]
    public Transform visualRoot;                 // empty child under the enemy where the model goes
    public Transform defaultProjectileSpawn;     // fallback if skin has no socket
    public string enemyLayerName = "Enemy";      // ensures all skin children get correct layer

    [HideInInspector] public Transform projectileSpawnPoint; // used by your AI script

    Transform _instancedModel;
    Animator _anim;

    public void ApplySkin(EnemySkinDefinition skin)
    {
        if (visualRoot == null) { Debug.LogError("EnemyVisual: visualRoot not set."); return; }

        // Clear old visuals
        for (int i = visualRoot.childCount - 1; i >= 0; i--)
            Destroy(visualRoot.GetChild(i).gameObject);

        // Instantiate model
        if (skin.modelPrefab == null) { Debug.LogError("EnemyVisual: skin has no modelPrefab."); return; }
        _instancedModel = Instantiate(skin.modelPrefab, visualRoot).transform;
        _instancedModel.localPosition  = skin.localPosition;
        _instancedModel.localEulerAngles = skin.localEuler;
        _instancedModel.localScale     = skin.localScale;

        // Ensure Enemy layer on all children
        int layer = LayerMask.NameToLayer(enemyLayerName);
        SetLayerRecursive(_instancedModel.gameObject, layer);

        // Animator (optional)
        _anim = _instancedModel.GetComponentInChildren<Animator>();
        if (_anim == null) _anim = _instancedModel.gameObject.AddComponent<Animator>();
        if (skin.controller) _anim.runtimeAnimatorController = skin.controller;
        if (skin.avatar)     _anim.avatar = skin.avatar;
        _anim.applyRootMotion = false;

        // Try to find a named socket on the skin; else use default; else auto-create
        projectileSpawnPoint = FindDeepChild(_instancedModel, skin.projectileSocketName) ?? defaultProjectileSpawn;

        if (projectileSpawnPoint == null)
        {
            // Auto-create a sensible forward socket based on mesh bounds
            var auto = new GameObject("AutoProjectileSocket").transform;
            auto.SetParent(visualRoot, false);

            // Try to place a bit in front of the visual's bounds center
            var rend = _instancedModel.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // Convert world center to visualRoot local space, then push forward
                var localCenter = visualRoot.InverseTransformPoint(rend.bounds.center);
                auto.localPosition = localCenter + Vector3.forward * 0.75f;
            }
            else
            {
                auto.localPosition = new Vector3(0f, 1.2f, 0.5f);
            }

            auto.localRotation = Quaternion.identity;
            projectileSpawnPoint = auto;
            Debug.LogWarning($"{name}: No projectile socket found; created fallback '{auto.name}'.");
        }
    }

    public Animator GetAnimator() => _anim;

    // ---------- helpers ----------
    static Transform FindDeepChild(Transform parent, string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform) SetLayerRecursive(t.gameObject, layer);
    }
}
