using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlotHandler : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public Image slotImg;
    public TextMeshProUGUI itemPrice;
    public InventoryManager inventoryManager;
    private UpgradeEventsController upgradeEventsController;

    void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        upgradeEventsController = UpgradeEventsController.GetUpgradeEventsController();
        
        if (item != null)
        {
            item = item.Clone();
            slotImg.sprite = item.itemImg;
            itemPrice.text = item.itemPrice.ToString();
        }
        else
        {
            itemPrice.text = string.Empty;
            slotImg.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Left click: try to buy/use the item in this slot.
        if (eventData.button != PointerEventData.InputButton.Left){
            Debug.Log("Not left click");
            return;
        }

        if (item == null){
            Debug.Log("No item in slot");
            return;
        }

        // Try to consume the item
        if (inventoryManager.TryConsume(this))
        {
            // If consumption was successful, trigger the upgrade event
            TriggerUpgrade();
        }
    }

    private void TriggerUpgrade()
    {
        if (item == null || upgradeEventsController == null)
            return;

        switch (item.upgradeType)
        {
            case UpgradeType.MovementSpeed:
                upgradeEventsController.TriggerMoveSpeedUpgrade();
                break;
            case UpgradeType.FireRate:
                upgradeEventsController.TriggerFireRateUpgrade();
                break;
            case UpgradeType.TapeGun:
                upgradeEventsController.TriggerTapeGunUpgrade();
                break;
            case UpgradeType.TruckMove:
                upgradeEventsController.TriggerTruckMoveUpgrade();
                break;
            case UpgradeType.None:
            default:
                break;
        }
    }
}
