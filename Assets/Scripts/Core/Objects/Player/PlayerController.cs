using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float CurHp = 100.0f;
    public float MaxHp = 100.0f;
    public float moveSpeed = 5.0f;
    public float rotSpeed = 2.0f;
}

public class PlayerController : MonoBehaviour
{
    private PlayerStats Stats;
    public Camera Cam;
    public GameObject Aim;
    public bool AimisActive = true;

    void Start()
    {
        Cursor.visible = false;
        Stats = new PlayerStats();
    }
    
    void Update()
    {
        MoveCtrl();
        RotCtrl();
    }

    void MoveCtrl()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.down * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * Stats.moveSpeed * Time.deltaTime);
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            AimisActive = !AimisActive;
            Aim.SetActive(AimisActive);
        }
    }

    void RotCtrl()
    {
        float rotX = Input.GetAxis("Mouse Y") * Stats.rotSpeed;
        float rotY = Input.GetAxis("Mouse X") * Stats.rotSpeed;

        transform.localRotation *= Quaternion.Euler(0, rotY, 0);
        Cam.transform.localRotation *= Quaternion.Euler(-rotX, 0, 0);
    }
}
