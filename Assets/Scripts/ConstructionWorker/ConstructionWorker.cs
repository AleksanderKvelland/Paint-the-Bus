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
        if (controller == null)
        {
            Debug.LogError("ConstructionWorker requires a CharacterController component!");
        }

        HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
        }
    }

    public void SetBusTarget(GameObject targetBus)
    {
        bus = targetBus;
        Debug.Log($"Construction worker set bus target: {targetBus.name}");
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (bus == null)
        {
            Debug.LogWarning("Bus target is null!");
            return;
        }

        if (controller == null)
        {
            Debug.LogError("CharacterController is null!");
            return;
        }

        // Calculate direction to bus
        Vector3 direction = (bus.transform.position - transform.position).normalized;
        
        // Move towards bus
        Vector3 movement = direction * speed * Time.deltaTime;
        controller.Move(movement);
        
        Debug.Log($"Moving towards bus. Current pos: {transform.position}, Bus pos: {bus.transform.position}");
    }
}
