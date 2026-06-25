using UnityEngine;

public class DestroyWhenFall : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, 1f);
        }
    }
}