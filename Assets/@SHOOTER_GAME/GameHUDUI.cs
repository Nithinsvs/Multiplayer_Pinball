using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: nithin.s
/// </summary>
public class GameHUDUI : MonoBehaviour
{
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private IWeapon[] _allWeapons;
    [SerializeField] private Text _currentAmmoCount;
    [SerializeField] private Text _totalAmmoCount;
    [SerializeField] private Image[] _allWeaponSelectedImages;

    private int currentWeapon;

    private void Awake()
    {
        currentWeapon = SaveGameData.LastSelectedWeapon;
    }

    private void OnEnable()
    {
        _weaponManager.PossessWeapon += ShowWeaponUI;
    }

    private void OnDisable()
    {
        _weaponManager.PossessWeapon -= ShowWeaponUI;
    }

    void Start()
    {
        ShowWeaponUI(currentWeapon);
    }

    private void ShowWeaponUI(int currentWeapon)
    {
        IWeapon weapon = _allWeapons[currentWeapon];

        _currentAmmoCount.text = weapon.AmmoCount.ToString();
        _totalAmmoCount.text = weapon.TotalAmmoCount.ToString();

        for (int i = 0; i < _allWeaponSelectedImages.Length; i++)
        {
            _allWeaponSelectedImages[i].enabled = (i == currentWeapon) ? true : false;
        }
    }
}
