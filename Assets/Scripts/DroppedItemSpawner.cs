using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] objectToBeSpawned;
    GameObject SpawnedObject;
    GameObject orientation;
    Rigidbody rb;

    public void SpawnDroppedItem(int itemid)
    {
        orientation = GameObject.Find("Orientation");
        SpawnedObject = Instantiate(objectToBeSpawned[itemid], orientation.transform.position, orientation.transform.rotation);
        SpawnedObject.transform.Translate(0, 0, 0.7f);
        rb = SpawnedObject.GetComponent<Rigidbody>();
        rb.AddForce(SpawnedObject.transform.forward * 0.3f, ForceMode.Impulse);
    }
}
