using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public int maxStackItems = 99;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    int selectedSlot = -1;
    int toolbarSlot = -1;
    [HideInInspector] public InventoryManager inventoryManager;

    [SerializeField] private GameObject[] objectToBeSpawned;
    [SerializeField] private InvItem[] invItems;
    GameObject SpawnedObject;
    GameObject orientation;
    Rigidbody rb;

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
            for (int i = 0; i < invItems.Length; i++)
            {
                if (invItems[i] == invItem)
                {
                    SpawnDroppedItem(i);
                }
            }
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

    public void SpawnDroppedItem(int itemid)
    {
        orientation = GameObject.Find("Orientation");
        string gameObjectName = objectToBeSpawned[itemid].name;
        SpawnedObject = PhotonNetwork.InstantiateRoomObject(Path.Combine("Prefabs", "Items", gameObjectName), orientation.transform.position, orientation.transform.rotation);
        SpawnedObject.name = SpawnedObject.name.Replace("(Clone)", "").Trim();
        SpawnedObject.transform.Translate(0, 0, 0.7f);
        rb = SpawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(SpawnedObject.transform.forward * 0.3f, ForceMode.Impulse);
    }

    public void CheckForAddItem(GameObject gameObject)
    {
        for (int i = 0; i < objectToBeSpawned.Length; i++)
        {
            if (objectToBeSpawned[i].name == gameObject.name)
            {
                AddItem(invItems[i]);
            }
        }
    }
}
