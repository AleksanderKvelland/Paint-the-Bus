using System;
using UnityEngine;


public class LootPickup : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = gameObject.GetComponent<InventoryManager>();
        if (inventoryManager == null)
            throw new Exception(
                $"The game object \"{gameObject.name}\" has a LootPickup component, but no InventoryManager " +
                "component. Add an InventoryManager component to it for LootPickup to work."
            );
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Loot"))
        {
            Destroy(other.gameObject);
            inventoryManager.money += 1;
        }
    }
}
