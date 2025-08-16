using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Set these in the Inspector or assign them in Start()
    public LayerMask EgroundLayer;
    public LayerMask EplayerLayer;

    private bool hasHit = false;

    /*
    private void OnCollisionEnter(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;

        if (((1 << otherLayer) & groundLayer) != 0 || ((1 << otherLayer) & playerLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
    */

    private void Start()
    {
        Destroy(gameObject, 5f); // Failsafe to clean up if it never hits anything
    }
    //Method didnt work, but will keep just in case
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return; // prevent double execution
        hasHit = true;

        int otherLayer = collision.gameObject.layer;

        if (((1 << otherLayer) & groundLayer) != 0 || ((1 << otherLayer) & playerLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
       
       if (hasHit) return;
        hasHit = true;

        int otherLayer = other.gameObject.layer;
        Debug.Log("Enemy projectile triggered with: " + other.name + " | Layer: " + LayerMask.LayerToName(otherLayer));

        if (((1 << otherLayer) & EplayerLayer) != 0)
        {
            // Try getting PlayerHealth from root (in case collider is on a child)
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Projectile hit the player!");
                playerHealth.TakeDamage(20);
            }
        }

        if (((1 << otherLayer) & EgroundLayer) != 0 || ((1 << otherLayer) & EplayerLayer) != 0)
        {
            Destroy(this.gameObject);
        }
    }
}
