using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "TPS Shooter/Weapons/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string weaponName;
    [SerializeField] private Sprite icon;

    [Header("Damage")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;

    [Header("Fire")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float spread = 0f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int reserveAmmo = 60;

    [Header("Reload")]
    [SerializeField] private float reloadDuration = 1.2f;

    [Header("Shotgun")]
    [SerializeField] private int pelletCount = 10;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileForce = 25f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;

    [Header("Melee")]
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float attackAngle = 90f;
    [SerializeField] private float backstabMultiplier = 2f;
    [SerializeField] private float knockbackForce = 5f;

    public string WeaponName => weaponName;
    public Sprite Icon => icon;

    public float Damage => damage;
    public float Range => range;
    public float FireRate => fireRate;
    public float Spread => spread;

    public int MagazineSize => magazineSize;
    public int ReserveAmmo => reserveAmmo;

    public float ReloadDuration => reloadDuration;

    public int PelletCount => pelletCount;

    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileForce => projectileForce;

    public float ExplosionRadius => explosionRadius;
    public float ExplosionForce => explosionForce;

    public float AttackRadius => attackRadius;
    public float AttackAngle => attackAngle;
    public float BackstabMultiplier => backstabMultiplier;
    public float KnockbackForce => knockbackForce;
}