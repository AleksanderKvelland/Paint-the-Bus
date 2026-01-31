using Combat;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private bool useRaycast = false; // Option for more accurate hit detection
    [SerializeField] private DamageType damageType = DamageType.Pierce;


    void Start()
    {
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Handle what happens when bullet hits something
        HandleHit(collision.gameObject, collision.GetContact(0).point);

        HealthComponent targetHealth = collision.gameObject.GetComponent<HealthComponent>();
        if (targetHealth != null)
        {
            targetHealth.Damage(damage, damageType);
        }
    }

    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        // Optional: Apply damage if the hit object has a health component
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Optional: Create hit effect at the impact point
        // Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

        // Despawn the bullet
        Destroy(gameObject);
    }
}

// Optional interface for objects that can take damage
public interface IDamageable
{
    void TakeDamage(float damage);
}