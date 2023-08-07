using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    public TextMeshProUGUI countText;
    bool draggin;
    [HideInInspector] public GameObject PlayerCanvas;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;
    public KeyCode ringInv = KeyCode.R;
    public KeyCode itemInv = KeyCode.Tab;

    [HideInInspector] public InvItem invItem;

    public void InitialiseItem(InvItem newItem)
    {
        invItem = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }
    void Start()
    {
        PlayerCanvas = GameObject.Find("Canvas");
    }
    void Update()
    {
        if (draggin == true && (Input.GetKeyDown(ringInv) || Input.GetKeyDown(itemInv)))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            draggin = false;
            eventData.pointerDrag = null;
        }
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
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
}
