using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Common refs")]
    public Transform firePoint;
    public Transform projectileParent;

    [Header("Input (HOTAS)")]
    public HotasWeaponInput input; // assign in Inspector

    [Header("Slots (IWeapon behaviours)")]
    public MonoBehaviour[] weaponBehaviours = new MonoBehaviour[3];

    [Header("Weapon View Models (Prefabs)")]
    public GameObject plasmaModelPrefab;
    public GameObject rapidModelPrefab;
    public GameObject batonModelPrefab;

    [Header("Model Anchors")]
    public Transform leftWeaponAnchor;
    public Transform rightWeaponAnchor;

    private GameObject currentRightModel;

    public int currentIndex { get; private set; } = 0;
    IWeapon[] weapons;
    IWeapon current;

    void Awake()
    {
        weapons = new IWeapon[weaponBehaviours.Length];
        for (int i = 0; i < weaponBehaviours.Length; i++)
            weapons[i] = weaponBehaviours[i] as IWeapon;

        // Equip first valid
        for (int i = 0; i < weapons.Length; i++)
            if (weapons[i] != null) { Equip(i); break; }
    }

    void Update()
    {
        HandleSwitchInput();

        if (current != null && input != null)
            current.Tick(input.TriggerHeld, input.TriggerDown, input.TriggerUp, Time.deltaTime);
    }

    void HandleSwitchInput()
    {
        if (input == null) return;

        if (input.Slot1Pressed) Equip(0);
        if (input.Slot2Pressed) Equip(1);
        if (input.Slot3Pressed) Equip(2);

        if (input.NextPressed) Next();
        if (input.PrevPressed) Prev();
    }

    public void Next() => Equip(FindNext(currentIndex, +1));
    public void Prev() => Equip(FindNext(currentIndex, -1));

    int FindNext(int start, int dir)
    {
        int n = weapons.Length;
        for (int i = 1; i <= n; i++)
        {
            int idx = (start + dir * i + n) % n;
            if (weapons[idx] != null) return idx;
        }
        return start;
    }

    public void Equip(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (weapons[index] == null) return;
        if (current == weapons[index]) return;

        // Deactivate current GameObject if possible
        if (weaponBehaviours[currentIndex] != null)
            weaponBehaviours[currentIndex].gameObject.SetActive(false);

        current?.OnUnequip();
        currentIndex = index;
        current = weapons[index];

        // Activate new one
        if (weaponBehaviours[currentIndex] != null)
            weaponBehaviours[currentIndex].gameObject.SetActive(true);

        current.OnEquip(firePoint, projectileParent);

        // Always show plasma (left hand) model if not already present
        if (plasmaModelPrefab != null && leftWeaponAnchor.childCount == 0)
        {
            var plasmaModel = Instantiate(plasmaModelPrefab, leftWeaponAnchor);
            plasmaModel.transform.localPosition = Vector3.zero;
            plasmaModel.transform.localRotation = Quaternion.identity;
        }

        // Replace right-hand weapon model
        if (currentRightModel != null)
            Destroy(currentRightModel);

        GameObject modelToSpawn = null;
        if (index == 0) modelToSpawn = rapidModelPrefab;
        else if (index == 2) modelToSpawn = batonModelPrefab;

        if (modelToSpawn != null)
        {
            currentRightModel = Instantiate(modelToSpawn, rightWeaponAnchor);
            currentRightModel.transform.localPosition = Vector3.zero;
            currentRightModel.transform.localRotation = Quaternion.identity;
        }
    }
}
