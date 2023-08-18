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
    [HideInInspector] public InventoryManager inventoryManager;

    private GameObject[] objectToBeSpawned;
    private InvItem[] invItems;
    GameObject SpawnedObject;
    GameObject orientation;
    Rigidbody rb;
    PhotonView PV;

    private void Start()
    {
        // Sets the first toolbar slot to active and loads all the items and their 3D gobjects.
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetInventoryManager(inventoryManager);
        }
        objectToBeSpawned = Resources.LoadAll<GameObject>("Prefabs/Items");
        invItems = Resources.LoadAll<InvItem>("Prefabs/Items");
        orientation = GameObject.Find("Orientation");
        PV = playerController.GetComponentInChildren<PhotonView>();
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
            playerController.ItemIsEquipped(false);
            ShowEquippedItem();
            return;
        }
        inventorySlots[newValue].Select();
        toolbarSlot = newValue;
        playerController.ItemIsEquipped(true);
        InventoryItem itemInSlot = inventorySlots[newValue].GetComponentInChildren<InventoryItem>();
        ShowEquippedItem();
        Debug.Log(itemInSlot);
    }

    public bool AddItem(InvItem invItem)
    {
        int minSlot = 3;
        if (invItem.type == ItemType.Equipment)
        {
            minSlot = 0;
        }
        //Process for adding/ creating a new item in your inventory, checks if there a stacakble item in inventory already and if not spawns it in.
        for (int i = minSlot; i < inventorySlots.Length; i++)
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
        for (int i = minSlot; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(invItem, slot);
                if (i < 3)
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
        string gameObjectName = objectToBeSpawned[itemid].name;
        SpawnedObject = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Items", gameObjectName), orientation.transform.position, orientation.transform.rotation);
        SpawnedObject.transform.Translate(0, 0, 0.7f);
        rb = SpawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(SpawnedObject.transform.forward * 0.3f, ForceMode.Impulse);
    }

    public void CheckForAddItem(GameObject gameObject, int playerID, int itemObjectViewId)
    {
        //Checks the names of th raycast object against each 3D object in the objectToBeSpawned array. Uses the same index for invItems array too to create an InventoryItem.
        for (int i = 0; i < objectToBeSpawned.Length; i++)
        {
            if ((objectToBeSpawned[i].name) == gameObject.name.Replace("(Clone)", "").Trim())
            {
                if (AddItem(invItems[i]))
                {
                    Debug.Log("Destroying object " + itemObjectViewId);
                    PV.RPC("RPC_PickupItem", RpcTarget.All, itemObjectViewId, playerID);
                }
                return;
            }
        }
    }

    public void ShowEquippedItem()
    {
        if (toolbarSlot <= -1)
        {
            playerController.EquipItem(-1);
            return;
        }

        InventorySlot slot = inventorySlots[toolbarSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot == null || itemInSlot.gameObject == null)
        {
            playerController.EquipItem(-1);
            return;
        }

        InvItem invItem = itemInSlot.invItem;
        for (int i = 0; i < invItems.Length; i++)
        {
            if (invItems[i] == invItem)
            {
                playerController.EquipItem(i);
                return;
            }
        }
    }
    IEnumerator WaitOneFrame()
    {
        yield return 0;
        ShowEquippedItem();
    }
}
