using UnityEngine;

public class ConstructionWorker : MonoBehaviour
{

    [SerializeField] private GameObject bus;
    [SerializeField] private float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, bus.transform.position, speed * Time.deltaTime);
    }
}
