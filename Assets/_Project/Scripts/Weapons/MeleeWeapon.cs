using UnityEngine;
using UnityEngine.Rendering;

public sealed class MeleeWeapon : WeaponBase
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private LayerMask hitMask;

    [Header("Visual")]
    [SerializeField] private float visualDuration = 0.2f;
    [SerializeField, Range(0.05f, 1f)] private float visualAlpha = 0.3f;

    private readonly Collider[] hitBuffer = new Collider[32];

    private GameObject visual;
    private Material visualMaterial;

    protected override void Awake()
    {
        base.Awake();
        CreateVisual();
    }

    public override void Fire()
    {
        if (attackPoint == null)
        {
            Debug.LogError(
                $"{name}: Attack Point is not assigned.",
                this);

            return;
        }

        Vector3 attackPosition =
            attackPoint.position;

        int hitCount =
            Physics.OverlapSphereNonAlloc(
                attackPosition,
                attackRadius,
                hitBuffer,
                hitMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = hitBuffer[i];

            if (hit == null)
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }
        }

        ShowVisual();
    }

    public override void Reload()
    {
        
    }

    private void CreateVisual()
    {
        visual = GameObject.CreatePrimitive(
            PrimitiveType.Sphere);

        visual.name = "MeleeAttackVisual";

        Collider collider =
            visual.GetComponent<Collider>();

        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer =
            visual.GetComponent<Renderer>();

        if (renderer == null)
            return;

        Shader shader =
            Shader.Find("Unlit/Color");

        if (shader == null)
        {
            Debug.LogError(
                "Shader Unlit/Color не найден.",
                this);

            return;
        }

        visualMaterial =
            new Material(shader);

     
        visualMaterial.SetColor(
            "_Color",
            new Color(
                1f,
                0f,
                0f,
                visualAlpha));

        
        visualMaterial.SetFloat(
            "_Mode",
            2f);

        visualMaterial.SetInt(
            "_SrcBlend",
            (int)BlendMode.SrcAlpha);

        visualMaterial.SetInt(
            "_DstBlend",
            (int)BlendMode.OneMinusSrcAlpha);

        visualMaterial.SetInt(
            "_ZWrite",
            0);

        visualMaterial.DisableKeyword(
            "_ALPHATEST_ON");

        visualMaterial.EnableKeyword(
            "_ALPHABLEND_ON");

        visualMaterial.DisableKeyword(
            "_ALPHAPREMULTIPLY_ON");

        visualMaterial.renderQueue =
            (int)RenderQueue.Transparent;

        renderer.material =
            visualMaterial;

        visual.SetActive(false);
    }

    private void ShowVisual()
    {
        if (visual == null ||
            attackPoint == null)
        {
            return;
        }

        visual.transform.position =
            attackPoint.position;

        visual.transform.rotation =
            attackPoint.rotation;

        visual.transform.localScale =
            Vector3.one *
            (attackRadius * 2f);

        visual.SetActive(true);

        CancelInvoke(
            nameof(HideVisual));

        Invoke(
            nameof(HideVisual),
            visualDuration);
    }

    private void HideVisual()
    {
        if (visual != null)
        {
            visual.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();

        if (visualMaterial != null)
        {
            Destroy(visualMaterial);
        }

        if (visual != null)
        {
            Destroy(visual);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color =
            new Color(1f, 0f, 0f, 0.3f);

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}