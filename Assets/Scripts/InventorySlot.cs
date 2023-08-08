using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;
    public int slotNumber;
    public InventoryManager inventoryManager;
    int selectedSlotTemp;

    public void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectedSlotTemp = inventoryManager.GiveSelectedSlot();
        inventoryManager.SetSelectedSlot(slotNumber);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManager.SetSelectedSlot(selectedSlotTemp);
    }

    public void SetInventoryManager(InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
    }
}
