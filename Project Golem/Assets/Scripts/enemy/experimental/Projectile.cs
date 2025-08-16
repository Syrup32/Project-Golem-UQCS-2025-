using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Set these in the Inspector or assign them in Start()
    public LayerMask groundLayer;
    public LayerMask playerLayer;

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

        if (((1 << otherLayer) & groundLayer) != 0 || ((1 << otherLayer) & playerLayer) != 0)
        {
            Debug.Log("Projectile hit: " + other.name);
            Debug.Log("Trigger Entered: " + other.name + " | Layer: " + LayerMask.LayerToName(other.gameObject.layer));

            Destroy(gameObject);
        }
    }
}
