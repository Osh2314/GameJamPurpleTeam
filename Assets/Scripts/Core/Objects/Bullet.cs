using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Manager.Sound;

public class Bullet : MonoBehaviourPunCallbacks
{
    PhotonView PV = null;
    Vector3 direction = Vector3.zero;

    public float damage = 1f;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Destroy(gameObject, 3.5f);
    }

    void Update()
    {
        direction.y = -GameObject.FindGameObjectWithTag("MainCamera").transform.localRotation.x + Input.GetAxisRaw("Mouse Y");
        transform.Translate(direction * 50f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Hunter")) PV.RPC("DestroyBulletRPC", RpcTarget.AllBuffered);
        if (!PV.IsMine && other.CompareTag("Ghost") && other.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<GamePlayer>().Hit(damage);
            PV.RPC("DestroyBulletRPC", RpcTarget.AllBuffered);
        }
        else if (PV.IsMine && other.CompareTag("Ghost") && !other.GetComponent<PhotonView>().IsMine)
        {
            SoundPlayer.instance.PlaySound("Bullet Hitting");
        }
    }

    [PunRPC]
    void SetBulletRPC(Vector3 direction, float damage)
    {
        this.direction = direction;
        this.damage = damage;
    }

    [PunRPC]
    void DestroyBulletRPC() => Destroy(gameObject);
}
