using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    public PlayerController owner;
    public bool Grounded;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            owner.grounded = true;
        }
        if (collision.CompareTag("Player"))
        {
            owner.groundedPlayer = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            owner.grounded = false;
        }
        if (collision.CompareTag("Player"))
        {
            owner.groundedPlayer = false;
        }
    }
}
