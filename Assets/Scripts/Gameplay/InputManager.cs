using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    [SerializeField] public Camera ConnectedCamera;
    [SerializeField][Range(0,1f)] private float CameraSmooth;
    public Character ConnectedCharacter;

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Vector2 MousePosition = ConnectedCamera.ScreenToWorldPoint(Input.mousePosition);
            ConnectedCharacter.PhotonView.RPC(nameof(ConnectedCharacter.HitMP), RpcTarget.All, MousePosition.x, MousePosition.y);
        }
    }

    private void FixedUpdate()
    {
        if (ConnectedCharacter)
        {
            Vector2 Direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            ConnectedCamera.transform.position = Vector3.Lerp(new Vector3(ConnectedCamera.transform.position.x,ConnectedCamera.transform.position.y, -10), new Vector3(ConnectedCharacter.transform.position.x, ConnectedCharacter.transform.position.y, -10) , CameraSmooth);
            ConnectedCharacter.AddMove(Direction);

            
        }    
    }

}
