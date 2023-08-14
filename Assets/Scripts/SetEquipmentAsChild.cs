using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetEquipmentAsChild : MonoBehaviourPun
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void DestroyComponents()
    {
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<PUN2_RigidbodySync>());
    }

    public void CallRPCSetAsChild()
    {
        var playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_SetAsChild", RpcTarget.All, playerID);
    }

    [PunRPC]
    void RPC_SetAsChild(int playerID)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            var equippedItemPosition = GameObject.Find("EquippedItemPosition");
            this.gameObject.transform.SetParent(equippedItemPosition.transform);
        }
    }
}
