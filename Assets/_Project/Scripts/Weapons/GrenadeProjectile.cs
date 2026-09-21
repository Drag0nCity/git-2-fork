using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private WeaponConfig config;
    [SerializeField] private LayerMask damageMask;

    [Header("Explosion View")]
    [SerializeField] private Color explosionColor = Color.red;
    [SerializeField] private float explosionLineWidth = 0.04f;
    [SerializeField] private float explosionDisplayTime = 0.5f;
    [SerializeField] private int circleSegments = 48;

    private readonly Collider[] _hitBuffer = new Collider[32];

    private Rigidbody _rigidbody;
    private Collider _collider;
    private bool _exploded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.collisionDetectionMode =
            CollisionDetectionMode.Continuous;

        if (config == null)
        {
            Debug.LogError(
                $"{name}: WeaponConfig is not assigned.",
                this);
        }
    }

    public void Launch(Vector3 direction)
    {
        if (config == null)
        {
            return;
        }

        _exploded = false;

        _rigidbody.velocity =
            direction.normalized *
            config.ProjectileForce;
    }

    public void IgnoreOwner(GameObject owner)
    {
        if (owner == null || _collider == null)
        {
            return;
        }

        Collider[] ownerColliders =
            owner.GetComponentsInChildren<Collider>();

        for (int i = 0; i < ownerColliders.Length; i++)
        {
            if (ownerColliders[i] == null)
            {
                continue;
            }

            Physics.IgnoreCollision(
                _collider,
                ownerColliders[i]);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        if (_exploded)
        {
            return;
        }

        _exploded = true;

        if (config == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 explosionPosition =
            transform.position;

        CreateExplosionVisual(
            explosionPosition);

        int hitCount =
            Physics.OverlapSphereNonAlloc(
                explosionPosition,
                config.ExplosionRadius,
                _hitBuffer,
                damageMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider target =
                _hitBuffer[i];

            if (target == null)
            {
                continue;
            }

            IDamageable damageable =
                target.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            damageable.TakeDamage(
                config.Damage);
        }

        Destroy(gameObject);
    }

    private void CreateExplosionVisual(
        Vector3 position)
    {
        GameObject visual =
            new GameObject(
                "Explosion Radius");

        visual.transform.position =
            position + Vector3.up * 0.05f;

        LineRenderer line =
            visual.AddComponent<LineRenderer>();

        line.useWorldSpace = false;

        line.positionCount =
            circleSegments + 1;

        line.startWidth =
            explosionLineWidth;

        line.endWidth =
            explosionLineWidth;

        line.material =
            new Material(
                Shader.Find("Sprites/Default"));

        line.startColor =
            explosionColor;

        line.endColor =
            explosionColor;

        for (int i = 0;
             i <= circleSegments;
             i++)
        {
            float angle =
                (float)i /
                circleSegments *
                Mathf.PI *
                2f;

            float x =
                Mathf.Cos(angle) *
                config.ExplosionRadius;

            float z =
                Mathf.Sin(angle) *
                config.ExplosionRadius;

            line.SetPosition(
                i,
                new Vector3(x, 0f, z));
        }

        Destroy(
            visual,
            explosionDisplayTime);
    }

    private void OnDestroy()
    {
        // Nothing to clean up.
    }
}