using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{

    public bool open = false;

    public GameObject wall;

    public GameObject door;
    public int roomlink { get; set; }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 2;
        Gizmos.DrawRay(transform.position, forward);
    }

    void Update()
    {
        if (open == true)
        {
            wall.SetActive(false);
            door.SetActive(true);
        }
        else
        {
            wall.SetActive(true);
            door.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Entrance")
        {
            open = true;
        }
    }
}
