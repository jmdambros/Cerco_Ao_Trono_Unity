using UnityEngine;

public class King : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health;
    public float directHitDamage = 60f;
    public float crushDamage = 30f;

    private bool isDead = false;

    void Awake()
    {
        health = maxHealth;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead) return;

        float impactForce = col.relativeVelocity.magnitude;
        if (impactForce < 2f) return;

        Troop troop = col.gameObject.GetComponent<Troop>();
        if (troop != null)
            TakeDamage(directHitDamage * (impactForce / 10f));
        else
            TakeDamage(crushDamage * (impactForce / 10f));
    }

    void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Max(health, 0f);
        Debug.Log($"King health: {health}");
        if (health <= 0f) Die();
    }

    void Die()
    {
        isDead = true;
        Debug.Log("THE KING IS DETHRONED!");
        GameManager.Instance.CheckForVictory();
        Destroy(gameObject, 0.5f);
    }
}