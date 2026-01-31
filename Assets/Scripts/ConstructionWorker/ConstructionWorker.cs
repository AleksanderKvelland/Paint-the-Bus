using UnityEngine;
using Combat;

public class ConstructionWorker : MonoBehaviour
{
    private GameObject bus;
    [SerializeField] private float speed = 5f;

    private void Awake()
    {
        HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
        healthComponent.OnDeath += HandleDeath;
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
            
        transform.position = Vector3.MoveTowards(transform.position, bus.transform.position, speed * Time.deltaTime);
    }
}
