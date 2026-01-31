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

    void Awake()
    {
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
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (item == null)
            return;

        inventoryManager.TryConsume(this);
    }
}
