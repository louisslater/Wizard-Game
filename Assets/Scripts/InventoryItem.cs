using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    bool draggin;
    [HideInInspector] public GameObject PlayerCanvas;
    [HideInInspector] public Transform parentAfterDrag;
    public KeyCode ringInv = KeyCode.R;
    public KeyCode itemInv = KeyCode.Tab;

    [HideInInspector] public InvItem invItem;

    public void InitialiseItem(InvItem newItem)
    {
        invItem = newItem;
        image.sprite = newItem.image;
    }
    void Start()
    {
        InitialiseItem(invItem);
        PlayerCanvas = GameObject.Find("Canvas");
    }
    void Update()
    {
        if (draggin == true && (Input.GetKeyDown(ringInv) || Input.GetKeyDown(itemInv)))
        {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(PlayerCanvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        draggin = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetKeyDown(ringInv) || Input.GetKeyDown(itemInv))
        {
            eventData.pointerDrag = null;
            return;
        }
        Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        draggin = false;
    }
}
