using UnityEngine;


//tropa mais simples, so herda de troop

public class Catapult : Troop
{
    [Header("Catapult Specific")]
    public float impactRadius = 0.5f;   // how wide the hit feels (cosmetic for now)

    protected override void OnHit(Collision2D col)
    {
        // Basic hit - just physics, no special effect
        // Later you can add a dust particle here
        Debug.Log($"Catapult hit {col.gameObject.name} at force {col.relativeVelocity.magnitude}");
    }
}