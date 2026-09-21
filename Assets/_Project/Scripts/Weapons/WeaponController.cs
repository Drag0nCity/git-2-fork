using UnityEngine;
using Zenject;

public sealed class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponBase[] weapons;
    [SerializeField] private int startingWeaponIndex;

    private IInputService _inputService;
    private WeaponBase _currentWeapon;
    private int _currentWeaponIndex;

    [Inject]
    private void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    private void Awake()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogError(
                $"{name}: Weapons are not assigned.",
                this);

            return;
        }

        _currentWeaponIndex = Mathf.Clamp(
            startingWeaponIndex,
            0,
            weapons.Length - 1);

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(
                    i == _currentWeaponIndex);
            }
        }

        _currentWeapon = weapons[_currentWeaponIndex];
    }

    private void Update()
    {
        if (_currentWeapon == null ||
            _inputService == null)
        {
            return;
        }

        if (_inputService.FirePressed)
        {
            _currentWeapon.Fire();
        }

        if (_inputService.ReloadPressed)
        {
            _currentWeapon.Reload();
        }

        HandleWeaponSelection();
    }

    private void HandleWeaponSelection()
    {
        int weaponSlot = _inputService.WeaponSlot;

        if (weaponSlot >= 0)
        {
            SelectWeapon(weaponSlot);
            return;
        }

        float scroll = _inputService.WeaponScroll;

        if (scroll > 0f)
        {
            SelectNextWeapon();
        }
        else if (scroll < 0f)
        {
            SelectPreviousWeapon();
        }
    }

    private void SelectNextWeapon()
    {
        int nextIndex =
            (_currentWeaponIndex + 1) %
            weapons.Length;

        SelectWeapon(nextIndex);
    }

    private void SelectPreviousWeapon()
    {
        int previousIndex =
            (_currentWeaponIndex - 1 +
             weapons.Length) %
            weapons.Length;

        SelectWeapon(previousIndex);
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 ||
            index >= weapons.Length)
        {
            return;
        }

        if (weapons[index] == null)
        {
            return;
        }

        if (index == _currentWeaponIndex)
        {
            return;
        }

        if (_currentWeapon != null)
        {
            _currentWeapon.gameObject.SetActive(false);
        }

        _currentWeaponIndex = index;
        _currentWeapon = weapons[index];

        _currentWeapon.gameObject.SetActive(true);
    }

    public WeaponBase CurrentWeapon =>
        _currentWeapon;

    public int CurrentWeaponIndex =>
        _currentWeaponIndex;
}