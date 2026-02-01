using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 1f;
    private UpgradeEventsController upgradeEventsController = UpgradeEventsController.GetUpgradeEventsController();
    private EquipmentController equipmentController;
    
    private float nextFireTime = 0f;
    private bool hasShot = false;

    void Start()
    {
        equipmentController = EquipmentController.GetEquipmentController();
        upgradeEventsController.onFireRateUpgrade += HandleFireRateUpgrade;
        equipmentController.onUse += Shoot;
    }

    void OnDestroy()
    {
        if (equipmentController != null)
        {
            equipmentController.onUse -= Shoot;
        }
        if (upgradeEventsController != null)
        {
            upgradeEventsController.onFireRateUpgrade -= HandleFireRateUpgrade;
        }
    }

    void Update()
    {
        if (hasShot)
        {
            if (Time.time >= nextFireTime)
            {
                hasShot = false;
            }
        }
    }

    void Shoot(GameObject gameObject)
    {
        if (gameObject != this.gameObject)
            return;
        Debug.Log("Shooting");
        if (hasShot || Time.time < nextFireTime)
            return;
        hasShot = true;
        nextFireTime = Time.time + fireRate;
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
        fireRate = Mathf.Max(0.1f, fireRate - 0.3f); // Decrease fire rate, minimum of 0.1 seconds
    }


}