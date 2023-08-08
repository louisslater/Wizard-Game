using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxStackItems = 99;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    int selectedSlot = -1;
    int toolbarSlot = -1;
    [HideInInspector] public InventoryManager inventoryManager;

    private void Start()
    {
        ChangeToolbarSlot(0);
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetInventoryManager(inventoryManager);
        }
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 3 && number < 7)
            {
                ChangeToolbarSlot(number - 4);
            }
        }
    }

    void ChangeToolbarSlot(int newValue)
    {
        if (toolbarSlot >= 0)
        {
            inventorySlots[toolbarSlot].Deselect();
        }
        if (toolbarSlot == newValue)
        {
            selectedSlot = -1;
            return;
        }

        inventorySlots[newValue].Select();
        toolbarSlot = newValue;
    }

    public bool AddItem(InvItem invItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.invItem == invItem && itemInSlot.count < maxStackItems && itemInSlot.invItem.stackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(invItem, slot);
                return true;
            }
        }
        return false;
    }
    void SpawnNewItem(InvItem invItem, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(invItem);
        inventoryItem.SetInventoryManager(inventoryManager);
    }

    public InvItem GetSelectedItem(bool use)
    {
        if (toolbarSlot < 0)
        {
            return null;
        }
        InventorySlot slot = inventorySlots[toolbarSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            InvItem invItem = itemInSlot.invItem;
            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return invItem;
        }
        return null;
    }

    public InvItem DropInvItem()
    {
        if (selectedSlot < 0)
        {
            return null;
        }
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            InvItem invItem = itemInSlot.invItem;
            itemInSlot.count--;
            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
            }
            else
            {
                itemInSlot.RefreshCount();
            }
            return invItem;
        }
        return null;
    }

    public void SetSelectedSlot(int slotNumber)
    {
        selectedSlot = slotNumber;
    }

    public int GiveSelectedSlot()
    {
        return selectedSlot;
    }
}
