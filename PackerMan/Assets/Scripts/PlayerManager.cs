using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerController playerOne;
    public PlayerController playerTwo;

    public void Update()
    {
        playerOne.horizontalAxis = Input.GetAxis("Horizontal");
        playerTwo.horizontalAxis = Input.GetAxis("HorizontalRight");

        playerOne.verticalAxis = Input.GetAxis("VerticalOne");
        playerTwo.verticalAxis = Input.GetAxis("VerticalTwo");

        playerOne.jumpButtonDown = Input.GetButtonDown("JumpOne");
        playerTwo.jumpButtonDown = Input.GetButtonDown("JumpTwo");

        playerOne.jumpButtonUp = Input.GetButtonUp("JumpOne");
        playerTwo.jumpButtonUp = Input.GetButtonUp("JumpTwo");

        playerOne.cancelButton = Input.GetButtonDown("Cancel");
        playerTwo.cancelButton = Input.GetButtonDown("Cancel");

        //print("H: " + playerOne.horizontalAxis);
        //print("V: " + playerOne.verticalAxis);

        float throwAxis = Input.GetAxis("ThrowOne");

        if (throwAxis < 0 || throwAxis > 0)
        {            
            playerOne.throwButton = true;
            playerTwo.throwButton = true;
        }
        else
        {            
            playerOne.throwButton = false;
            playerTwo.throwButton = false;
        }

        //if(throwAxis > 0)
        //{
        //    playerTwo.throwButton = true;
        //    playerOne.throwButton = true;
        //}
        //else
        //{
        //    playerTwo.throwButton = false;
        //    playerOne.throwButton = false;
        //}
            
        
        //print(Input.GetAxis("ThrowTwo"));
    }
}
