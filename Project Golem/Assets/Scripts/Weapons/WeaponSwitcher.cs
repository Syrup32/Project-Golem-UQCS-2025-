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

        current?.OnUnequip();
        currentIndex = index;
        current = weapons[index];
        current.OnEquip(firePoint, projectileParent);
    }
}
