using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Author: nithin.s
/// </summary>
public class ShootingManager : MonoBehaviour
{
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private InputAction _shootInputAction;
    [SerializeField] private Transform _hand_Bone;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private int _reduceByAmount = 1;

    private int _currentWeapon;

    private void OnEnable()
    {
        _weaponManager.PossessWeapon += HandleWeapon;
        _shootInputAction.performed += OnShootPerformed;
    }

    private void OnDisable()
    {
        _weaponManager.PossessWeapon -= HandleWeapon;
        _shootInputAction.performed -= OnShootPerformed;
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        weapons[_currentWeapon].GetComponent<IWeapon>().ReduceAmmo(_reduceByAmount);
    }

    void HandleWeapon(int weaponToUse)
    {
        _hand_Bone.position = weapons[weaponToUse].transform.position;
        weapons[weaponToUse].transform.SetParent(_hand_Bone.transform);
    }
}
