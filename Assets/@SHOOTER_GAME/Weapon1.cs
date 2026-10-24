using UnityEngine;

/// <summary>
/// Author: nithin.s
/// </summary>
public class Weapon1 : MonoBehaviour, IWeapon
{
    private int _ammo = 10;

    public int WeaponIndex { get => 1; }
    public int WeaponPowerupLevel { get => 1; }

    public int AmmoCount
    {
        get => _ammo;
        private set { _ammo = value; }
    }

    public int TotalAmmoCount { get => 10; }

    public void ReduceAmmo(int reduceAmount)
    {
        _ammo -= reduceAmount;
    }
}
