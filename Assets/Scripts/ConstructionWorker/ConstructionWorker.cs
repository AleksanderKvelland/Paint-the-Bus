using UnityEngine;
using Combat;

public class ConstructionWorker : MonoBehaviour
{
    private GameObject bus;
    [SerializeField] private float speed = 5f;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
        }
    }

    public void SetBusTarget(GameObject targetBus)
    {
        bus = targetBus;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (bus == null)
            return;

        if (controller == null)
            return;

        // Calculate direction to bus
        Vector3 direction = (bus.transform.position - transform.position).normalized;
        
        // Move towards bus
        Vector3 movement = direction * speed * Time.deltaTime;
        controller.Move(movement);
    }
}
