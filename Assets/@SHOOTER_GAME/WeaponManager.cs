using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Author: nithin.s
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public event Action<int> PossessWeapon;

    [SerializeField] private InputAction _switchWeaponInputAction;
    private int _currentWeapon;
    private const int MAX_WEAPONS = 4;

    private void OnEnable()
    {
        _switchWeaponInputAction.Enable();
        _switchWeaponInputAction.performed += OnWeaponSwitched;
    }

    private void OnDisable()
    {
        _switchWeaponInputAction.performed -= OnWeaponSwitched;
    }

    private void OnWeaponSwitched(InputAction.CallbackContext obj)
    {
        _currentWeapon = SaveGameData.LastSelectedWeapon;

        if (_currentWeapon >= MAX_WEAPONS - 1)
            _currentWeapon = -1;

        SelectWeapon(_currentWeapon++);
    }

    void Start()
    {
        SelectWeapon(_currentWeapon);
    }

    private void SelectWeapon(int weaponToUse)
    {
        PossessWeapon?.Invoke(weaponToUse);
        _currentWeapon = weaponToUse;
        SaveGameData.LastSelectedWeapon = weaponToUse;
    }
}
