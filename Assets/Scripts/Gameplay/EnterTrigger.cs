using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTrigger : MonoBehaviour
{
    [SerializeField] private Transform TeleportPosition;
    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character>())
        {
            Character Character = collision.GetComponent<Character>();
            if(Character.PhotonView.IsMine && Character.CanEnterRoom)
            {
                Character.transform.position = TeleportPosition.position;
                Character.InputManager.ConnectedCamera.transform.position = new Vector3(TeleportPosition.position.x,TeleportPosition.position.y, Character.InputManager.ConnectedCamera.transform.position.z);
                Character.CanEnterRoom = false;
            }
        }
    }
}
