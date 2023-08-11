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

    private GameObject[] objectToBeSpawned;
    private InvItem[] invItems;
    GameObject SpawnedObject;
    GameObject orientation;
    Rigidbody rb;

    private void Start()
    {
        // Sets the first toolbar slot to active and loads all the items and their 3D gobjects.
        ChangeToolbarSlot(0);
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetInventoryManager(inventoryManager);
        }
        objectToBeSpawned = Resources.LoadAll<GameObject>("Prefabs/Items");
        invItems = Resources.LoadAll<InvItem>("Prefabs/Items");
    }

    private void Update()
    {
        //Simply checks if the buttons to change slots are being pressed.
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
        //Changes the colour to red of the selected toolbar slot
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
        //Process for adding/ creating a new item in your inventory, checks if there a stacakble item in inventory already and if not spawns it in.
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
        //Adds a new instance of InventoryItem (new draggable item spawned)
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(invItem);
        inventoryItem.SetInventoryManager(inventoryManager);
    }

    public InvItem GetSelectedItem(bool use)
    {
        //Returns the item if its being "got", returns AND deletes the item if its being "used". Method only currently run in DemoScript (the inv buttons on the side)
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
        //Similar method to the GetSelectedItem() but always deletes the item and spawns a 3D object too. Used in InventoryItem when items are being dropped.
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
        //Each slot runs this method when the pointer hovers over it. This way InventoryManager knows from which slot in the inventorySlots array to delete/reduce the items from.
        selectedSlot = slotNumber;
    }

    public void SpawnDroppedItem(int itemid)
    {
        //Method that creates a 3D object from a given item in objectToBeSpawned array. The itemid correlate to the invItems index, which are both ordered by alphabetical order.
        orientation = GameObject.Find("Orientation");
        string gameObjectName = objectToBeSpawned[itemid].name;
        SpawnedObject = PhotonNetwork.InstantiateRoomObject(Path.Combine("Prefabs", "Items", gameObjectName), orientation.transform.position, orientation.transform.rotation);
        SpawnedObject.transform.Translate(0, 0, 0.7f);
        rb = SpawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(SpawnedObject.transform.forward * 0.3f, ForceMode.Impulse);
    }

    public void CheckForAddItem(GameObject gameObject)
    {
        //Checks the names of th raycast object against each 3D object in the objectToBeSpawned array. Uses the same index for invItems array too to create an InventoryItem.
        for (int i = 0; i < objectToBeSpawned.Length; i++)
        {
            if ((objectToBeSpawned[i].name + "(Clone)") == gameObject.name)
            {
                AddItem(invItems[i]);
                return;
            }
        }
    }
}
