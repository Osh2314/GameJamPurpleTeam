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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Stats = new PlayerStats();
    }
    
    // Update is called once per frame
    void Update()
    {
        MoveCtrl();
        RotCtrl();
    }

    void MoveCtrl()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            this.transform.Translate(Vector3.down * Stats.moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            this.transform.Translate(Vector3.up * Stats.moveSpeed * Time.deltaTime);
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

        this.transform.localRotation *= Quaternion.Euler(0, rotY, 0);
        Cam.transform.localRotation *= Quaternion.Euler(-rotX, 0, 0);
    }
}
