using UnityEngine;
// corre pra frente e explode
// diferente das outras, sem calculo de rota, so vai reto
public class UnicornKamikaze : Troop
{
    [Header("Kamikaze Specific")]
    public float explosionRadius = 2f;
    public float explosionForce = 20f;
    public float chargeSpeed = 14f;
    public GameObject explosionEffectPrefab;

    protected override void Update()
    {
        if (launched) return;
        if (!IsReadyToLaunch) return;

        if (Input.GetMouseButtonDown(0))
            Launch(Vector2.right * chargeSpeed);
    }

    protected override void Launch(Vector2 velocity)
    {
        launched = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f;
        rb.linearVelocity = velocity;
    }

    protected override void OnHit(Collision2D col)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
            if (hitRb == null || hitRb == rb) continue;

            Vector2 direction = (hit.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float forceFalloff = 1f - (distance / explosionRadius);
            hitRb.AddForce(direction * explosionForce * forceFalloff, ForceMode2D.Impulse);
        }

        Destroy(gameObject, 0.1f);
    }

#if UNITY_EDITOR
    // Override the base Gizmos draw to show explosion radius instead
    new void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}