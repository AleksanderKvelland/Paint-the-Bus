using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryGrid;
    public bool messyInventory;

    [Header("Money")]
    public int money;

    public TextMeshProUGUI priceText;

    private void Awake()
    {
        ConfigureInventory();
        RefreshMoneyUI();
    }

    public void RefreshMoneyUI()
    {
        if (priceText != null)
            priceText.text = money.ToString();
    }

    public UISlotHandler FindSlotWithItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID) || inventoryGrid == null)
            return null;

        for (int i = 0; i < inventoryGrid.transform.childCount; i++)
        {
            UISlotHandler slot = inventoryGrid.transform.GetChild(i).GetComponent<UISlotHandler>();
            if (slot != null && slot.item != null && slot.item.itemID == itemID)
                return slot;
        }

        return null;
    }

    public void PlaceInInventory(UISlotHandler activeSlot, Item item)
    {
        // Enforce "only one of each item" across the whole inventory.
        if (item != null)
        {
            UISlotHandler existing = FindSlotWithItem(item.itemID);
            if (existing != null && existing != activeSlot)
                return;
        }

        activeSlot.item = item;
        activeSlot.slotImg.sprite = item.itemImg;
        activeSlot.itemPrice.text = item.itemPrice.ToString();
        activeSlot.slotImg.gameObject.SetActive(true);
        ConfigureInventory();
    }

    /// <summary>
    /// Attempts to consume the item in the slot by paying its price.
    /// If there is enough money, money is reduced and the slot is cleared.
    /// </summary>
    public bool TryConsume(UISlotHandler activeSlot)
    {
        if (activeSlot == null || activeSlot.item == null)
            return false;

        int price = activeSlot.item.itemPrice;
        if (money < price)
            return false;

        money -= price;
        RefreshMoneyUI();

        ClearItemSlot(activeSlot);
        ConfigureInventory();
        return true;
    }

    public void ClearItemSlot(UISlotHandler activeSlot)
    {
        activeSlot.slotImg.sprite = null;
        activeSlot.slotImg.gameObject.SetActive(false);
        activeSlot.itemPrice.text = string.Empty;
        activeSlot.item = null;
    }

    public void ConfigureInventory()
    {
        if (messyInventory) { return; }

        List<Transform> uiSlots = new List<Transform>();
        for (int i = 0; i < inventoryGrid.transform.childCount; i++)
        {
            uiSlots.Add(inventoryGrid.transform.GetChild(i));
        }

        uiSlots.Sort((a, b) =>
        {
            UISlotHandler itemA = a.GetComponent<UISlotHandler>();
            UISlotHandler itemB = b.GetComponent<UISlotHandler>();

            bool hasItemA = itemA.item != null;
            bool hasItemB = itemB.item != null;

            return hasItemB.CompareTo(hasItemA);
        });

        for (int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetSiblingIndex(i);
        }
    }
}
