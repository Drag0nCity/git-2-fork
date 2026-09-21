using UnityEngine;

public sealed class AmmoPickup : MonoBehaviour, IPickup
{
    [SerializeField] private int ammoAmount = 30;

    public void Pickup(GameObject picker)
    {
        WeaponController weaponController =
            picker.GetComponentInChildren<WeaponController>();

        if (weaponController == null)
        {
            return;
        }

        WeaponBase weapon =
            weaponController.CurrentWeapon;

        if (weapon == null)
        {
            return;
        }

        weapon.AddAmmo(ammoAmount);

        Destroy(gameObject);
    }
}