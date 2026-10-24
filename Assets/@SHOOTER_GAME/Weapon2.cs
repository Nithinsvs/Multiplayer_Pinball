using UnityEngine;

/// <summary>
/// Author: nithin.s
/// </summary>
public class Weapon2 : MonoBehaviour, IWeapon
{
    private int _ammo = 20;

    public int WeaponIndex { get => 2; }
    public int WeaponPowerupLevel { get => 1; }

    public int AmmoCount
    {
        get => _ammo;
        private set { _ammo = value; }
    }

    public int TotalAmmoCount { get => 20; }

    public void ReduceAmmo(int reduceAmount)
    {
        _ammo -= reduceAmount;
    }
}
