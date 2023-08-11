using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;
    public int slotNumber;
    private InventoryManager inventoryManager;

    public void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        //Changes the colour of the slot when its selected in the toolbar, NOT if its being hovered over!
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Checks if the dragging InventoryItem can be dropped onto the slot. If it can then the slot becomes the parent of the Inventory item and the item moves positions.
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Sends the selected slot to the inventoryManager on pointer entry. Used specifically for the dropping of items where the slot position in array is needed.
        inventoryManager.SetSelectedSlot(slotNumber);
    }

    public void SetInventoryManager(InventoryManager inventoryManager)
    {
        //Method is run on start and assigns the Player's inventoryManager to be the one to send the selected slot values to.
        this.inventoryManager = inventoryManager;
    }
}
