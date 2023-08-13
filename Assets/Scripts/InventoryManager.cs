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
    public PlayerController playerController;
    int selectedSlot = -1;
    int toolbarSlot = -1;
    int previousValue;
    [HideInInspector] public InventoryManager inventoryManager;

    private GameObject[] objectToBeSpawned;
    private InvItem[] invItems;
    GameObject SpawnedObject;
    GameObject orientation;
    GameObject equippedItemPosition;
    GameObject[] equippedObjects;
    Rigidbody rb;

    private void Start()
    {
        // Sets the first toolbar slot to active and loads all the items and their 3D gobjects.
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetInventoryManager(inventoryManager);
        }
        objectToBeSpawned = Resources.LoadAll<GameObject>("Prefabs/Items");
        equippedObjects = objectToBeSpawned;
        invItems = Resources.LoadAll<InvItem>("Prefabs/Items");
        orientation = GameObject.Find("Orientation");
        equippedItemPosition = GameObject.Find("EquippedItemPosition");

        int j = 0;
        foreach (GameObject gobject in objectToBeSpawned)
        {
            equippedObjects[j] = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Items", gobject.name), equippedItemPosition.transform.position, equippedItemPosition.transform.rotation);
            equippedObjects[j].transform.SetParent(equippedItemPosition.transform);
            equippedObjects[j].SetActive(false);
            Debug.Log("Created equipment  " + equippedObjects[j]);
            Destroy(equippedObjects[j].GetComponent<Rigidbody>());
            Destroy(equippedObjects[j].GetComponent<Collider>());
            ++j;
        }
        ChangeToolbarSlot(0);
    }

    public void ChangeToolbarSlot(int newValue)
    {
        //Changes the colour to red of the selected toolbar slot
        if (toolbarSlot >= 0)
        {
            inventorySlots[toolbarSlot].Deselect();
        }
        if (toolbarSlot == newValue)
        {
            toolbarSlot = -1;
            ShowEquippedItem();
            return;
        }
        inventorySlots[newValue].Select();
        toolbarSlot = newValue;
        InventoryItem itemInSlot = inventorySlots[newValue].GetComponentInChildren<InventoryItem>();
        ShowEquippedItem();
        Debug.Log(itemInSlot);
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
                if (i <= 2)
                {
                    playerController.FadeToolbar();
                    ShowEquippedItem();
                }
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

    public void DropInvItem()
    {
        //Similar method to the GetSelectedItem() but always deletes the item and spawns a 3D object too. Used in InventoryItem when items are being dropped.
        if (selectedSlot < 0)
        {
            return;
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
                    break;
                }
            }
            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
                Debug.Log("Equipping item now!");
                Debug.Log(itemInSlot.gameObject);
                StartCoroutine(WaitOneFrame());
            }
            else
            {
                itemInSlot.RefreshCount();
            }
            return;
        }
    }

    public void SetSelectedSlot(int slotNumber)
    {
        //Each slot runs this method when the pointer hovers over it. This way InventoryManager knows from which slot in the inventorySlots array to delete/reduce the items from.
        selectedSlot = slotNumber;
    }

    public void SpawnDroppedItem(int itemid)
    {
        //Method that creates a 3D object from a given item in objectToBeSpawned array. The itemid correlate to the invItems index, which are both ordered by alphabetical order.
        string gameObjectName = objectToBeSpawned[itemid].name.Replace("(Clone)", "").Trim();
        SpawnedObject = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Items", gameObjectName), orientation.transform.position, orientation.transform.rotation);
        SpawnedObject.transform.Translate(0, 0, 0.7f);
        rb = SpawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(SpawnedObject.transform.forward * 0.3f, ForceMode.Impulse);
    }

    public void CheckForAddItem(GameObject gameObject)
    {
        //Checks the names of th raycast object against each 3D object in the objectToBeSpawned array. Uses the same index for invItems array too to create an InventoryItem.
        for (int i = 0; i < objectToBeSpawned.Length; i++)
        {
            if ((objectToBeSpawned[i].name) == gameObject.name)
            {
                AddItem(invItems[i]);
                return;
            }
        }
    }

    public void ShowEquippedItem()
    {
        if (toolbarSlot <= -1)
        {
            foreach (GameObject gobject in equippedObjects)
            {
                gobject.SetActive(false);
            }
            return;
        }

        InventorySlot slot = inventorySlots[toolbarSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot == null || itemInSlot.gameObject == null)
        {
            foreach (GameObject gobject in equippedObjects)
            {
                gobject.SetActive(false);
            }
            return;
        }
        InvItem invItem = itemInSlot.invItem;
        for (int i = 0; i < invItems.Length; i++)
        {
            equippedObjects[i].SetActive(false);
            if (invItems[i] == invItem)
            {
                Debug.Log("Equipping item " + equippedObjects[i]);
                equippedObjects[i].SetActive(true);
            }
        }
    }
    IEnumerator WaitOneFrame()
    {
        yield return 0;
        ShowEquippedItem();
    }
}
