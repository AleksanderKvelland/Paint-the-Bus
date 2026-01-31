using UnityEngine;

public class ConstructionWorker : MonoBehaviour
{
    private GameObject bus;
    [SerializeField] private float speed = 5f;

    public void SetBusTarget(GameObject targetBus)
    {
        bus = targetBus;
    }

    void Update()
    {
        if (bus == null)
            return;
            
        transform.position = Vector3.MoveTowards(transform.position, bus.transform.position, speed * Time.deltaTime);
    }
}
