using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: nithin.s
/// </summary>
public interface IWeapon
{
    public int WeaponIndex { get; }
    public int WeaponPowerupLevel { get; }
    public int AmmoCount { get; }
    public int TotalAmmoCount { get; }
    public void ReduceAmmo(int reduceAmount);
}
