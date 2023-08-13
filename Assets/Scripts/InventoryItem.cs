using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //This be the ol' script for the draggable item you see in the inventory with your own glassy eye, me-hearties.

    public Image image;
    public TextMeshProUGUI countText;
    [HideInInspector] public InventoryManager inventoryManager;
    bool draggin;
    bool hovering;
    [HideInInspector] public GameObject PlayerCanvas;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;
    public KeyCode ringInv = KeyCode.R;
    public KeyCode itemInv = KeyCode.Tab;
    public KeyCode dropItem = KeyCode.G;

    [HideInInspector] public InvItem invItem;

    public void InitialiseItem(InvItem newItem)
    {
        //Creates a new InventoryItem with a sprite.
        invItem = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }
    void Start()
    {
        //Finds the canvas so it can be set as the parent as the InventoryItem is being dragged (so its on top of everything).
        PlayerCanvas = GameObject.Find("Canvas");
        draggin = false;
        hovering = false;
    }
    void Update()
    {
        //If an inventory button is clicked when draggin then the InventoryItem is sent back to its original slot.
        if (draggin == true && (Input.GetKeyDown(ringInv) || Input.GetKeyDown(itemInv)))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            draggin = false;
            eventData.pointerDrag = null;
            return;
        }
        //If the drop key is clicked while the mouse is "hovering" above the InventoryItem
        if (draggin == false && hovering && Input.GetKeyDown(dropItem))
        {
            inventoryManager.DropInvItem();
        }
    }

    public void SetInventoryManager(InventoryManager inventoryManager)
    {
        //Same as in InventorySlot, the InventoryItem assigns the inventoryManager as its own.
        this.inventoryManager = inventoryManager;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(PlayerCanvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        draggin = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (draggin == true)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggin == true)
        {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            draggin = false;
        }
    }

    public void RefreshCount()
    {
        //Turns off the InventoryItem count if its more than 1, pretty simple really.
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
