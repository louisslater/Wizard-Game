using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    public Animator camAnim;
    public PlayerController playerController;
    float horizontalInput;
    float verticalInput;
    bool landed = false;
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if ((horizontalInput != 0 || verticalInput != 0) && playerController.IsGrounded() && landed == false)
        {
            camAnim.ResetTrigger("idle");
            camAnim.ResetTrigger("landing");
            camAnim.SetTrigger("walk");
        }
        else if (landed == false)
        {
            camAnim.ResetTrigger("walk");
            camAnim.ResetTrigger("landing");
            camAnim.SetTrigger("idle");
        }
    }
    public void PlayerHeadbobLanded()
    {
        camAnim.ResetTrigger("idle");
        camAnim.ResetTrigger("walk");
        camAnim.SetTrigger("landing");
        landed = true;
        StartCoroutine(Landed());
    }

    IEnumerator Landed()
    {
        yield return new WaitForSeconds(0.25f);
        landed = false;
    }
}
