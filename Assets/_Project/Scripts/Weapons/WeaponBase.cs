using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponConfig config;

    protected WeaponConfig Config => config;

    public string WeaponName =>
        config != null ? config.WeaponName : string.Empty;

    public Sprite Icon =>
        config != null ? config.Icon : null;

    public float Damage =>
        config != null ? config.Damage : 0f;

    public float Range =>
        config != null ? config.Range : 0f;

    public float FireRate =>
        config != null ? config.FireRate : 0f;

    public float Spread =>
        config != null ? config.Spread : 0f;

    public int MagazineSize =>
        config != null ? config.MagazineSize : 0;

    public int ReserveAmmo =>
        config != null ? config.ReserveAmmo : 0;

    private int currentReserveAmmo;

    protected virtual void Awake()
    {
        if (config == null)
        {
            Debug.LogError(
                $"{name}: WeaponConfig is not assigned.",
                this);

            return;
        }

        currentReserveAmmo = config.ReserveAmmo;
    }

    public abstract void Fire();

    public virtual void Reload()
    {
    }

    public void AddAmmo(int amount)
    {
        currentReserveAmmo += amount;

        if (currentReserveAmmo < 0)
        {
            currentReserveAmmo = 0;
        }
    }

    public int CurrentReserveAmmo =>
        currentReserveAmmo;
}