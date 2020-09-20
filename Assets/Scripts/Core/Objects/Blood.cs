using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Blood : MonoBehaviourPunCallbacks
{
    PhotonView PV = null;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Destroy(gameObject, 3.5f);
    }
}
