using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 2;
        Gizmos.DrawRay(transform.position, forward);
    }
}
