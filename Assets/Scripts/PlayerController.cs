using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPunOwnershipCallbacks
{
    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity;

    [SerializeField] Item[] items;

    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] [Min(1)] private float hitrange = 3;
    private RaycastHit hit;

    int itemIndex;
    int previousItemIndex = -1;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    public GameObject RingInventoryGroup;
    public GameObject ItemInventoryGroup;
    public GameObject ToolbarInventory;
    public GameObject crosshair;
    [SerializeField] private InventoryManager inventoryManager;

    PlayerManager playerManager;

    float verticalLookRotation;
    public float moveSpeed;
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
    public float airMultiplier;
    bool readyToJump;
    bool canMove;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode ringInv = KeyCode.R;
    public KeyCode itemInv = KeyCode.Tab;
    public KeyCode interactKey = KeyCode.F;

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

            EquipItem(0);

            var FPSView = GameObject.Find("FPSView");
            FPSView.gameObject.layer = LayerMask.NameToLayer("FPSView");

            var children = FPSView.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = LayerMask.NameToLayer("FPSView");
            }
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
            rb.drag = 0;

        if (canMove)
        {
            MyInput();
            Look();
            SpeedControl();

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

            // Shoots a raycast and checks if the interaction key is down. Disables the object that is hit, deletes it for all players and adds the item into your inventory.
            if (Input.GetKeyDown(interactKey) && Physics.Raycast(cameraHolder.transform.position, cameraHolder.transform.forward, out hit, hitrange, pickableLayerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out PhotonView pv))
                {
                    hit.collider.gameObject.SetActive(false);
                    var itemObjectViewId = pv.ViewID;
                    PV.RPC("RPC_PickupItem", RpcTarget.All, itemObjectViewId);
                }
            }
        }

        if (transform.position.y <= -10f)
        {
            Die();
        }
    }

    [PunRPC]
    void RPC_PickupItem(int viewId)
    {
        //Sends a message to everybody dat the 3D gobject is deleted and adds the item into the inventory.
        var itemObjectView = PhotonNetwork.GetPhotonView(viewId);
        itemObjectView.TransferOwnership(PhotonNetwork.LocalPlayer);
        inventoryManager.CheckForAddItem(itemObjectView.gameObject);
        PhotonNetwork.Destroy(itemObjectView.gameObject);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        throw new System.NotImplementedException();
    }

    void Look()
    {
        //Changes camera rotation
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;


        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if(previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
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
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
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

    public void WhenRingInventoryButtonClicked()
    {
        //Peforms all the actions needed when the specific inventory button is pressed (like disabling mouse and crosshair and etc).
        ItemInventoryGroup.SetActive(false);
        if (RingInventoryGroup.activeInHierarchy == true)
        {
            RingInventoryGroup.SetActive(false);
            ToolbarInventory.SetActive(true);
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
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            crosshair.SetActive(true);
        }
        else
        {
            ItemInventoryGroup.SetActive(true);
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
    }
}