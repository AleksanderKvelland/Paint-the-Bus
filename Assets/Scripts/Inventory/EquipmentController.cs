using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentController : MonoBehaviour
{
    [SerializeField] private Transform handSocket;
    private GameObject currentObject;
    private InputAction attackAction;
    public event Action<GameObject> onUse;
    private static EquipmentController EquipmentControllerInstance;
    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    public static EquipmentController GetEquipmentController()
    {
        if (EquipmentControllerInstance == null)
        {
            EquipmentControllerInstance = FindFirstObjectByType<EquipmentController>();
        }
        return EquipmentControllerInstance;
    }

    public void Equip(InventoryItem item)
    {
        Unequip();

        if (item.itemPrefab == null)
            return;
        
        currentObject = Instantiate(item.itemPrefab,
            handSocket.position,
            handSocket.rotation,
            handSocket
        );
    }

    public void Unequip()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
            currentObject = null;
        }
    }

    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            onUse?.Invoke(currentObject);
        }
    }
}
