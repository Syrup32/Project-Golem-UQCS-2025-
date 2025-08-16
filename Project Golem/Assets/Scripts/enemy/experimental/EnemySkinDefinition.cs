using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Skin", fileName = "EnemySkin_")]
public class EnemySkinDefinition : ScriptableObject
{
    [Header("Model")]
    public GameObject modelPrefab;                 // FBX/Prefab with renderers, colliders, (optional) Animator
    public Vector3 localPosition;                  // default (0,0,0)
    public Vector3 localEuler;                     // default (0,0,0)
    public Vector3 localScale = Vector3.one;       // tweak per skin

    [Header("Animation (optional)")]
    public RuntimeAnimatorController controller;   // if the prefab doesn't already include one
    public Avatar avatar;                          // for Humanoid; leave null for Generic

    [Header("Sockets (names on the model)")]
    public string projectileSocketName = "ProjectileSocket"; // empty child on model you can place in FBX/prefab
    public string leftFootName = "Foot_L";                   // optional for stomp SFX
    public string rightFootName = "Foot_R";

    [Header("Audio (optional)")]
    public AudioClip footstepClip;
}
