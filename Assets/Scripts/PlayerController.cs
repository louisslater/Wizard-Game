using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("Camera")]
    [SerializeField] GameObject cameraHolder;
    public Headbob camAnim;
    [SerializeField] float mouseSensitivity;

    [Header("Inventory")]
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] [Min(1f)] private float hitrange = 3;
    private RaycastHit hit;
    [SerializeField] GameObject[] items;
    int itemIndex;
    int previousItemIndex = -1;
    int selectedSlot = 0;
    bool itemIsEquipped;
    public GameObject RingInventoryGroup;
    public GameObject ItemInventoryGroup;
    public GameObject ToolbarInventory;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("UI")]
    [SerializeField] [Min(0.1f)] float fadeInSpeed = 3;
    [SerializeField] [Min(0.1f)] float fadeOutSpeed = 1;
    [SerializeField] [Min(0f)] float fadeWait = 2;
    private FadeUI toolbarUI;
    public GameObject crosshair;

    [Header("Physicsy stuff")]
    public float moveSpeed;
    float verticalLookRotation;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float jumpPenalty;
    float timeUntilJump = 0;
    public float airMultiplier;
    bool readyToJump;
    [HideInInspector] public bool canMove;
    PlayerManager playerManager;
    Rigidbody rb;
    PhotonView PV;

    [Header("Key Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode ringInv = KeyCode.R;
    public KeyCode itemInv = KeyCode.Tab;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode dropHeldEquipment = KeyCode.G;

    [Header("Other")]
    const float maxHealth = 100f;
    float currentHealth = maxHealth;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        readyToJump = true;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ItemInventoryGroup.SetActive(false);
        RingInventoryGroup.SetActive(false);
        canMove = true;
        if(PV.IsMine)
        {
            gameObject.tag = "Player";

            toolbarUI = ToolbarInventory.GetComponentInChildren<FadeUI>();
            var FPSView = GameObject.Find("FPSView");
            FPSView.gameObject.layer = LayerMask.NameToLayer("FPSView");

            var children = FPSView.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = LayerMask.NameToLayer("FPSView");
            }
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<Canvas>().gameObject);
            Destroy(GetComponentInChildren<InventoryManager>().gameObject);
            Destroy(rb);
        }
    }

    void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(ringInv) && grounded)
        {
            WhenRingInventoryButtonClicked();
        }
        if (Input.GetKeyDown(itemInv) && grounded)
        {
            WhenItemInventoryButtonClicked();
        }

        if (grounded)
            rb.drag = groundDrag;
        else
        {
            rb.drag = 0;
        }

        if (timeUntilJump <= 0)
        {
            ResetJump();
        }
        timeUntilJump -= Time.deltaTime;

        if (canMove)
        {
            MyInput();
            Look();
            SpeedControl();

            if (Input.inputString != null)
            {
                bool isNumber = int.TryParse(Input.inputString, out int number);
                if (isNumber && number > 3 && number < 7)
                {
                    inventoryManager.ChangeToolbarSlot(number - 4);
                    selectedSlot = number - 4;
                    toolbarUI.ShowThenHideUI(fadeWait, fadeInSpeed, fadeOutSpeed);
                }
            }

            if (Input.GetKeyDown(dropHeldEquipment) && itemIsEquipped)
            {
                inventoryManager.SetSelectedSlot(selectedSlot);
                inventoryManager.DropInvItem();
            }

            /*
            for (int i = 0; i < items.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    EquipItem(i);
                    break;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (itemIndex >= items.Length - 1)
                {
                    EquipItem(0);
                }
                else
                {
                    EquipItem(itemIndex + 1);
                }
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (itemIndex <= 0)
                {
                    EquipItem(items.Length - 1);
                }
                else
                {
                    EquipItem(itemIndex - 1);
                }
            }

            if (Input.GetMouseButton(0))
            {
                items[itemIndex].Use();
            }
            */

            // Shoots a raycast and checks if the interaction key is down. Disables the object that is hit, deletes it for all players and adds the item into your inventory.
            if (Input.GetKeyDown(interactKey) && Physics.Raycast(cameraHolder.transform.position, cameraHolder.transform.forward, out hit, hitrange, pickableLayerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out PhotonView pv))
                {
                    var itemObjectViewId = pv.ViewID;
                    var playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                    inventoryManager.CheckForAddItem(hit.collider.gameObject, playerID, itemObjectViewId);
                    //PV.RPC("RPC_PickupItem", RpcTarget.All, itemObjectViewId, playerID);
                }
            }
        }

        if (transform.position.y <= -10f)
        {
            Die();
        }
    }

    [PunRPC]
    void RPC_PickupItem(int viewId, int playerID)
    {
        //Sends a message to everybody dat the 3D gobject is deleted and adds the item into the inventory.
        var itemObjectView = PhotonNetwork.GetPhotonView(viewId);
        itemObjectView.gameObject.SetActive(false);
        itemObjectView.TransferOwnership(playerID);
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            StartCoroutine(WaitingToDestroy(2f, itemObjectView.gameObject));
        }
    }

    void Look()
    {
        //Changes camera rotation
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    public void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        foreach (GameObject gobject in items)
        {
            gobject.SetActive(false);
        }
        itemIndex = _index;

        if (_index != -1)
        {
            items[itemIndex].SetActive(true);
        }

        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (canMove)
        {
            MovePlayer();
        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
            return;
        Debug.Log("took damage" + damage);

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            timeUntilJump = jumpCooldown;
            readyToJump = false;
            Jump();
            //Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x * jumpPenalty, 0f, rb.velocity.z * jumpPenalty);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    public void StartJumpCooldown()
    {
        timeUntilJump = jumpCooldown;
        readyToJump = false;
        camAnim.PlayerHeadbobLanded();
    }

    public void WhenRingInventoryButtonClicked()
    {
        //Peforms all the actions needed when the specific inventory button is pressed (like disabling mouse and crosshair and etc).
        ItemInventoryGroup.SetActive(false);
        if (RingInventoryGroup.activeInHierarchy == true)
        {
            RingInventoryGroup.SetActive(false);
            ToolbarInventory.SetActive(true);
            toolbarUI.HideUI(fadeOutSpeed);
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            crosshair.SetActive(true);
        }
        else
        {
            RingInventoryGroup.SetActive(true);
            ToolbarInventory.SetActive(false);
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
    }
    public void WhenItemInventoryButtonClicked()
    {
        //Also peforms all the actions needed when the specific inventory button is pressed
        ToolbarInventory.SetActive(true);
        RingInventoryGroup.SetActive(false);
        if (ItemInventoryGroup.activeInHierarchy == true)
        {
            inventoryManager.SetSelectedSlot(-1);
            ItemInventoryGroup.SetActive(false);
            toolbarUI.HideUI(fadeOutSpeed);
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            crosshair.SetActive(true);
        }
        else
        {
            ItemInventoryGroup.SetActive(true);
            toolbarUI.ShowUI(100);
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
    }

    IEnumerator WaitingToDestroy(float sec, GameObject itemObject)
    {
        // Waits for "sec" seconds
        yield return new WaitForSeconds(sec);
        PhotonNetwork.Destroy(itemObject);
    }

    public void FadeToolbar()
    {
        if (!ItemInventoryGroup.activeInHierarchy)
        {
            toolbarUI.HideUI(fadeOutSpeed);
        }
    }

    public void ItemIsEquipped(bool equipped)
    {
        itemIsEquipped = equipped;
    }
}