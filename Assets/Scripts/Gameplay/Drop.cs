using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Drop : MonoBehaviour
{
    [PunRPC]
    public void Take()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
