using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerOne;
    public Transform playerTwo;
    public Transform [] players;
    public Transform Package;

    public float smooth;
    public float minZoom;
    public float maxZoom;
    public float yPos;
    private Vector3 refVelocity = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 newPosition;

    public PlayerId currentPlayer;

    void Start()
    {
        newPosition = new Vector3(0, yPos, minZoom);
    }
    
    void FixedUpdate()
    {
        if(currentPlayer == PlayerId.playerTwo)
        {
            Vector3 cameraPosition = (playerOne.position + playerTwo.position + Package.position) / 3;

            cameraPosition.z = Vector3.Distance(playerOne.position, playerTwo.position);
            cameraPosition.z *= -1;
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, maxZoom, minZoom);
            //print(cameraPosition.z);
            cameraPosition.y += yPos;
            //newPosition.x = Mathf.SmoothDamp(newPosition.x, target.position.x, ref refVelocity.x, smooth * Time.deltaTime);
            //transform.position = cameraPosition;       
            //transform.position = Vector3.Lerp(transform.position, cameraPosition, smooth * Time.deltaTime);
            cameraPosition.x = Mathf.Round(cameraPosition.x);
            cameraPosition.y = Mathf.Round(cameraPosition.y);
            cameraPosition.z = Mathf.Round(cameraPosition.z);
            transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref refVelocity, smooth * Time.fixedDeltaTime);
        }
        else
        {
            Vector3 cameraPosition = playerOne.position;
            cameraPosition.y += yPos;
            cameraPosition.z = minZoom;
            transform.position = Vector3.SmoothDamp(transform.position, cameraPosition,ref refVelocity, smooth * Time.fixedDeltaTime);
            //transform.position = cameraPosition;
        }
    }

    void CameraPosition()
    {

        //Vector3 novaPosicao = Vector3.zero;
        //if (currentState != States.afterCut && !joysitick)
        //{
        //    camX += Input.GetAxis("MouseX") * Time.deltaTime * cameraSensitivy;
        //    camY += Input.GetAxis("MouseY") * Time.deltaTime * cameraSensitivy;
        //}
        //else if (currentState != States.afterCut && joysitick)
        //{
        //    camX += Input.GetAxis("J1_HorizontalRight") * Time.deltaTime * cameraSensitivy * 10;
        //    camY += Input.GetAxis("J1_VerticalRight") * Time.deltaTime * cameraSensitivy * 10;

        //}

        //CameraStates();

        //if (currentState != States.dead && currentState != States.afterCut)
        //{

        //    novaPosicao.x = Mathf.SmoothDamp(transform.position.x, player[playerIndex].position.x + camX, ref velocidade.x, smooth * Time.deltaTime);
        //    novaPosicao.y = Mathf.SmoothDamp(transform.position.y, player[playerIndex].position.y + camY, ref velocidade.y, smooth * Time.deltaTime);
        //    novaPosicao.z = Mathf.SmoothDamp(transform.position.z, zoom, ref velocidade.z, smoothZoom * Time.deltaTime);
        //}
        //else if (currentState == States.dead)
        //{
        //    novaPosicao.x = Mathf.SmoothDamp(transform.position.x, player[playerIndex].position.x + camX, ref velocidade.x, 40 * Time.deltaTime);
        //    novaPosicao.y = Mathf.SmoothDamp(transform.position.y, player[playerIndex].position.y + camY, ref velocidade.y, 40 * Time.deltaTime);
        //    novaPosicao.z = Mathf.SmoothDamp(transform.position.z, player[playerIndex].position.z - 3f, ref velocidade.z, 120 * Time.deltaTime);
        //}
        //else if (currentState == States.afterCut)
        //{
        //    novaPosicao.x = Mathf.SmoothDamp(transform.position.x, player[playerIndex].position.x + 3f, ref velocidade.x, 40 * Time.deltaTime);
        //    novaPosicao.y = Mathf.SmoothDamp(transform.position.y, player[playerIndex].position.y + 4, ref velocidade.y, 40 * Time.deltaTime);
        //    novaPosicao.z = Mathf.SmoothDamp(transform.position.z, zoom - 3, ref velocidade.z, 100 * Time.deltaTime);
        //}

        //float distance = transform.position.x - player[playerIndex].position.x;

        //FinalPos = novaPosicao;
        //transform.position = Vector3.Slerp(transform.position, FinalPos, Time.deltaTime * 60);

    }

}