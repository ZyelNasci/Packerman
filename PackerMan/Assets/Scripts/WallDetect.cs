using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetect : MonoBehaviour
{
    public PlayerController Owner;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            //Owner.currentVelocity.x = 0;
            Owner.wall = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            //Owner.currentVelocity.x = 0;
            Owner.wall = false;
        }
    }
}
