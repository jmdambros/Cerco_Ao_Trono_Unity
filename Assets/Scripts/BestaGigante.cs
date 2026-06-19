using UnityEngine;

public class BestaGigante : Troop
{
    [Header("Besta Settings")]
    public GameObject arrowPrefab;
    public int arrowCount = 3;
    public float spreadAngle = 15f;
    public float arrowForce = 20f;

    protected override void Launch(Vector2 velocity)
    {
        launched = true;
        HideDots();

        if (arrowPrefab == null)
        {
            Debug.LogError("BestaGigante: arrowPrefab not assigned!");
            Destroy(gameObject);
            return;
        }

        float baseAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (spreadAngle * (arrowCount - 1) / 2f);

        for (int i = 0; i < arrowCount; i++)
        {
            float angle = startAngle + spreadAngle * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            
            // strip the Troop script so the arrow doesn't try to act as a troop
            Troop troopScript = arrow.GetComponent<Troop>();
            if (troopScript != null) Destroy(troopScript);

            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.constraints = RigidbodyConstraints2D.None;
                arrowRb.gravityScale = 1f;
                arrowRb.linearVelocity = dir * arrowForce;
            }
        }

        Destroy(gameObject);
    }
}