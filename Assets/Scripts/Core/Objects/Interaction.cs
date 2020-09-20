using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Interaction : MonoBehaviourPunCallbacks
{
    GameObject playerObj = null;
    PhotonView PV = null;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        playerObj = GameObject.FindGameObjectWithTag("Ghost");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Aim"))
        {
            if (playerObj != null)
                playerObj.GetComponent<GamePlayer>().myStats.isInt = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Aim"))
        {
            if (playerObj != null)
                playerObj.GetComponent<GamePlayer>().myStats.isInt = false;
        }
    }

    public void InteractionObj()
    {
        if (name.Contains("Clock"))
        {
            Main.instance.goalCost++;
            if (Main.instance.goalCost >= Main.instance.userList.Count - 1)
            {
                Main.instance.PV.RPC("SetWinner", RpcTarget.AllBuffered, Main.State.GhostWin);
            }
            Destroy(gameObject);
        }
        else if (name.Contains("Knight"))
        {
            if (PV.IsMine) 
                transform.Find("Object004").rotation *= Quaternion.Euler(0f, 25f, 0f);
        }
        else if (name.Contains("Candle"))
        {
            PV.RPC("SetCandle", RpcTarget.AllBuffered, !transform.Find("FX_Fire_Small_03").gameObject.activeSelf);
        }
    }

    [PunRPC]
    void SetCandle(bool active)
    {
        Debug.Log(transform.Find("FX_Fire_Small_03").gameObject);
        transform.Find("FX_Fire_Small_03").gameObject.SetActive(active);
    }
}
