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

        // Clear old
        for (int i = visualRoot.childCount - 1; i >= 0; i--)
            Destroy(visualRoot.GetChild(i).gameObject);

        // Instantiate model
        if (skin.modelPrefab == null) { Debug.LogError("EnemyVisual: skin has no modelPrefab."); return; }
        _instancedModel = Instantiate(skin.modelPrefab, visualRoot).transform;
        _instancedModel.localPosition = skin.localPosition;
        _instancedModel.localEulerAngles = skin.localEuler;
        _instancedModel.localScale = skin.localScale;

        // Ensure layer on all children
        int layer = LayerMask.NameToLayer(enemyLayerName);
        SetLayerRecursive(_instancedModel.gameObject, layer);

        // Animator hookup (optional)
        _anim = _instancedModel.GetComponentInChildren<Animator>();
        if (_anim == null) _anim = _instancedModel.gameObject.AddComponent<Animator>();
        if (skin.controller) _anim.runtimeAnimatorController = skin.controller;
        if (skin.avatar) _anim.avatar = skin.avatar;
        _anim.applyRootMotion = false; // NavMeshAgent drives movement

        // Projectile socket
        projectileSpawnPoint = FindDeepChild(_instancedModel, skin.projectileSocketName) ?? defaultProjectileSpawn;

        // Optional: cache foot sockets if you want stomp sync later
        // var footL = FindDeepChild(_instancedModel, skin.leftFootName);
        // var footR = FindDeepChild(_instancedModel, skin.rightFootName);
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
