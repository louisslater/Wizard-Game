using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public InvItem[] itemsToPickup;

    public void PickUpItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickup[id]);
        if (result == true)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("Item NOT added");
        }
    }

    public void GetSelectedItem()
    {
        InvItem receivedItem = inventoryManager.GetSelectedItem(false);
        if (receivedItem != null)
        {
            Debug.Log("Received " + receivedItem);
        }
        else
        {
            Debug.Log("Not received");
        }
    }

    public void UseSelectedItem()
    {
        InvItem receivedItem = inventoryManager.GetSelectedItem(true);
        if (receivedItem != null)
        {
            Debug.Log("Used " + receivedItem);
        }
        else
        {
            Debug.Log("Not used");
        }
    }
}
