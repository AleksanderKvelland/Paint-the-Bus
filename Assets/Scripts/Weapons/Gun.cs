using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;
    private InputAction fireAction;
    private UpgradeEventsController upgradeEventsController = UpgradeEventsController.GetUpgradeEventsController();
    
    private float nextFireTime = 0f;

    void Start()
    {
        // Initialize input action for firing
        fireAction = InputSystem.actions.FindAction("Attack");
        upgradeEventsController.onFireRateUpgrade += HandleFireRateUpgrade;

    }

    void Update()
    {
        // Check for shoot input (left mouse button or custom key)
        if (fireAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        upgradeEventsController.TriggerMoveSpeedUpgrade();
        // Instantiate the bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Get the Rigidbody component and add velocity
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }

    void HandleFireRateUpgrade()
    {
        fireRate = Mathf.Max(0.1f, fireRate - 0.1f); // Decrease fire rate, minimum of 0.1 seconds
    }


}