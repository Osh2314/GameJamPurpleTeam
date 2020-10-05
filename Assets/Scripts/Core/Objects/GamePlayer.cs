using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Manager.Sound;
using Good;

public class GamePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum Kind
    {
        Hunter1, Hunter2, Hunter3, Hunter4,
        Ghost,
        NONE
    }

    [System.Serializable]
    public class Gun
    {
        public string name = "Test Gun";
        public float damage = 0f;
        public float attackSpeed = 1f, reloadSpeed = 1f;
        public int curAmmo = 0, maxAmmo = 25;
        public bool isFire = false, isReload = false;
    }

    [System.Serializable]
    public class Stats
    {
        public Kind isKind = Kind.NONE;
        public float curHP = 100f, maxHP = 100f;
        public bool isGame = false, isMove = false, isJump = false, isRunning = false, isInt = false, isDestroy = false, isGet = false, setChar = false, isSkill = false;
        public float moveSpeed = 5f, rotSpeed = 5f;
        public float getTime = 0f;

        public PhotonView PV = null;
        public Rigidbody Rigid = null;
        public Animator animtor = null;
        public GameObject Cam = null;
        public GameObject myLight = null;
        public GameObject ghostLight = null;
        public GameObject myEffect = null;
        public GameObject myCharacter = null;
        public Gun myGun = new Gun();
    }
    public Stats myStats = new Stats();
    public GameObject newCamera = null;

    public int curKind = 0;

    public bool curMove = false;
    public bool curJump = false;
    public bool curFire = false;
    public bool curReload = false;

    #region Game UI
    Image gunImage = null;
    Text  gunText = null;
    Text  ammoText = null;
    Text  goalText = null;
    Text  ggoalText = null;

    Text hpText = null;
    Image hpImage = null;
    GameObject interactionText = null;
    #endregion

    void Start()
    {
        if (myStats.PV.IsMine)
        {
            myStats.isKind = (Kind)Random.Range(0, 4);

            gunImage = GameObject.Find("GunImage").GetComponent<Image>();
            gunText = GameObject.Find("GunText").GetComponent<Text>();
            ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
            goalText = GameObject.Find("GoalText").GetComponent<Text>();
            ggoalText = GameObject.Find("GGoalText").GetComponent<Text>();

            hpText = GameObject.Find("GHPText").GetComponent<Text>();
            hpImage = GameObject.Find("GHPImage").GetComponent<Image>();
            interactionText = GameObject.FindGameObjectWithTag("UI");

            switch (myStats.isKind)
            {
                case Kind.Hunter1:
                    myStats.myGun.name = "Mini-Uzi";
                    myStats.myGun.damage = 0.5f;
                    myStats.myGun.attackSpeed = 0.05f;
                    myStats.myGun.reloadSpeed = 1.0f;
                    myStats.myGun.curAmmo = myStats.myGun.maxAmmo = 25;
                    break;
                case Kind.Hunter2:
                    myStats.myGun.name = "AK-47";
                    myStats.myGun.damage = 1.5f;
                    myStats.myGun.attackSpeed = 0.25f;
                    myStats.myGun.reloadSpeed = 1.5f;
                    myStats.myGun.curAmmo = myStats.myGun.maxAmmo = 30;
                    break;
                case Kind.Hunter3:
                    myStats.myGun.name = "Winchester M1897";
                    myStats.myGun.damage = 10f;
                    myStats.myGun.attackSpeed = 1.5f;
                    myStats.myGun.reloadSpeed = 0.5f;
                    myStats.myGun.curAmmo = myStats.myGun.maxAmmo = 5;
                    break;
                case Kind.Hunter4:
                    myStats.myGun.name = "Garand M1";
                    myStats.myGun.damage = 25f;
                    myStats.myGun.attackSpeed = 3.5f;
                    myStats.myGun.reloadSpeed = 2.5f;
                    myStats.myGun.curAmmo = myStats.myGun.maxAmmo = 8;
                    break;
                default:
                    break;
            }
            gunText.text = myStats.myGun.name;

            myStats.myCharacter = transform.GetChild((int)myStats.isKind).gameObject;
            myStats.animtor = myStats.myCharacter.GetComponent<Animator>();

            myStats.Cam = GameObject.FindGameObjectWithTag("MainCamera");
            myStats.Cam.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 0.5f);
            myStats.Cam.transform.parent = transform;

            myStats.myLight = GameObject.Find("Spot Light");
            myStats.ghostLight = GameObject.Find("GhostLight");
            myStats.myLight.SetActive(false);
            myStats.ghostLight.SetActive(false);

            myStats.PV.RPC("AddPlayer", RpcTarget.AllBuffered);
        }
        else
        {
            myStats.myCharacter = transform.GetChild(curKind).gameObject;
            if (curKind != 4)
                myStats.animtor = myStats.myCharacter.GetComponent<Animator>();
        }
    }

    void FixedUpdate()
    {
        if (myStats.PV.IsMine)
        {
            if (myStats.isGame)
            {
                if (myStats.isMove)
                {
                    Move();
                }
                Rotate();
            }
        }
    }

    void Update()
    {
        if (myStats.PV.IsMine)
        {
            if (myStats.isGame)
            {
                if (myStats.isKind != Kind.Ghost)
                {
                    GhostUI.instance.HideScreen();
                    HunterUI.instance.ShowScreen();
                    Fire();
                    Reload();

                    ammoText.text = myStats.myGun.curAmmo + " / " + myStats.myGun.maxAmmo;

                    if (Input.GetKeyDown(KeyCode.Q) && !Main.instance.LightBroken)
                    {
                        if (myStats.myLight.activeSelf)
                            myStats.myLight.SetActive(false);
                        else
                            myStats.myLight.SetActive(true);
                    }
                    else if (Main.instance.LightBroken)
                    {
                        myStats.myLight.SetActive(false);
                    }

                    gameObject.tag = "Hunter";
                    goalText.text = "유령을 찾아서 처치하세요 !";
                    gunText.text = myStats.myGun.name;
                }
                else
                {
                    HunterUI.instance.HideScreen();
                    GhostUI.instance.ShowScreen();

                    ggoalText.text = Main.instance.goalCost.ToString() + " / " + (Main.instance.userList.Count - 1).ToString();
                    hpText.text = "+" + myStats.curHP;
                    hpImage.fillAmount = (myStats.curHP - 0) / (myStats.maxHP - 0);

                    myStats.Rigid.useGravity = false;
                    gameObject.tag = "Ghost";
                    goalText.text = "?를 전부 찾으세요 !";

                    if (Input.GetKeyDown(KeyCode.E) && !Main.instance.LightBroken)
                    {
                        Main.instance.PV.RPC("LightB", RpcTarget.All);
                    }

                    if (Input.GetKey(KeyCode.Q) && myStats.isInt)
                    {
                        myStats.getTime += Time.deltaTime;
                        if (myStats.getTime >= 2f)
                        {
                            myStats.isInt = false;
                            myStats.getTime = 0f;
                            //GameObject.FindGameObjectWithTag("Interaction").GetComponent<Interaction>().InteractionObj();
                            MathG.FindNearTarget("Interaction", gameObject).GetComponent<Interaction>().InteractionObj();
                        }
                    }
                    else if (Input.GetKeyUp(KeyCode.Q) && myStats.isInt)
                    {
                        myStats.getTime = 0f;
                    }

                    if (interactionText != null)
                    {
                        if (myStats.isInt)
                        {
                            interactionText.gameObject.SetActive(true);
                        }
                        else
                        {
                            interactionText.gameObject.SetActive(false);
                        }
                    }
                }
            }
            if (myStats.curHP <= 0)
            {
                myStats.isMove = false;
                Main.instance.PV.RPC("SetWinner", RpcTarget.AllBuffered, Main.State.HunterWin);
                Main.instance.winnerObj.SetActive(true);
                DeadGhost();
            }
            if (myStats.isKind == Kind.Ghost && Main.instance.gameState == Main.State.GhostWin)
            {
                Main.instance.winnerObj.SetActive(true);
            }
        }
        else
        {
            if (myStats.isKind == Kind.Ghost)
            {
                myStats.Rigid.useGravity = false;
                gameObject.tag = "Ghost";

                myStats.myCharacter.SetActive(false);
                myStats.myCharacter = transform.GetChild(curKind).gameObject;
                myStats.myCharacter.SetActive(true);
                if (curKind != 4)
                    myStats.animtor = myStats.myCharacter.GetComponent<Animator>();
            }
            else
            {
                gameObject.tag = "Hunter";
                if (myStats.myGun.isFire)
                    myStats.myEffect.SetActive(true);
                else
                    myStats.myEffect.SetActive(false);

                if (myStats.animtor != null)
                {
                    myStats.animtor.SetBool("isMove", curMove);
                    myStats.animtor.SetBool("isJump", curJump);
                    myStats.animtor.SetBool("isFire", curFire);
                    myStats.animtor.SetBool("isReload", curReload);
                }

                if (myStats.setChar)
                {
                    myStats.myCharacter.SetActive(false);
                    myStats.myCharacter = transform.GetChild(curKind).gameObject;
                    myStats.myCharacter.SetActive(true);
                    if (curKind != 4)
                        myStats.animtor = myStats.myCharacter.GetComponent<Animator>();
                    myStats.setChar = false;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (Main.instance.gameState == Main.State.NONE)
        {
            if (newCamera != null)
                Instantiate(newCamera);
        }
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.Z) || transform.position.y >= 8.5f)
        {
            myStats.Rigid.velocity = Vector3.zero;
            myStats.Rigid.AddForce(Vector3.down * myStats.moveSpeed, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * myStats.moveSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * myStats.moveSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * myStats.moveSpeed * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * myStats.moveSpeed * Time.fixedDeltaTime);
        }

        if (myStats.isKind != Kind.Ghost) // 사냥꾼
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                myStats.animtor.SetBool("isMove", true);
                myStats.isRunning = true;
            }
            else
            {
                myStats.animtor.SetBool("isMove", false);
                myStats.isRunning = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && !myStats.isJump)
            {
                myStats.Rigid.AddForce(Vector3.up * myStats.moveSpeed, ForceMode.Impulse);
                myStats.animtor.SetBool("isJump", true);
                myStats.isJump = true;
            }
        }
        else // 유령
        {
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * myStats.moveSpeed * Time.fixedDeltaTime);
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                transform.Translate(Vector3.down * myStats.moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void Rotate()
    {
        float rotX = Input.GetAxisRaw("Mouse Y") * myStats.rotSpeed, rotY = Input.GetAxisRaw("Mouse X") * myStats.rotSpeed;

        transform.localRotation *= Quaternion.Euler(0, rotY, 0f);
        myStats.Cam.transform.localRotation *= Quaternion.Euler(-rotX, 0, 0f);
    }

    void Fire()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !myStats.myGun.isFire && !myStats.myGun.isReload)
        {
            if (myStats.myGun.curAmmo > 0)
            {
                // 총 쏘는 사운드
                SoundPlayer.instance.PlaySound("GunFire");
                StartCoroutine(FireCheck(myStats.myGun.attackSpeed));
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !myStats.myGun.isReload && !myStats.myGun.isFire)
        {
            SoundPlayer.instance.PlaySound("Reload.");
            StartCoroutine(ReloadCheck(myStats.myGun.attackSpeed));
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (myStats.PV.IsMine && other.gameObject.CompareTag("Ground"))
        {
            myStats.isJump = false;
            if (myStats.animtor != null)
                myStats.animtor.SetBool("isJump", false);
        }
    }

    IEnumerator FireCheck(float t = 1f)
    {
        myStats.myEffect.SetActive(true);
        myStats.myGun.curAmmo--;
        myStats.myGun.isFire = true;
        myStats.isMove = false;
        myStats.animtor.SetBool("isFire", true);
        PhotonNetwork.Instantiate("Bullet", new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity).GetComponent<PhotonView>().RPC("SetBulletRPC", RpcTarget.All, Quaternion.Euler(myStats.Cam.transform.localRotation.x, -myStats.Cam.transform.localRotation.y, 0f) * transform.forward, myStats.myGun.damage);
        yield return new WaitForSeconds(t);
        myStats.myEffect.SetActive(false);
        myStats.myGun.isFire = false;
        myStats.isMove = true;
        myStats.animtor.SetBool("isFire", false);
    }

    IEnumerator ReloadCheck(float t = 1f)
    {
        myStats.myGun.isReload = true;
        myStats.isMove = false;
        myStats.animtor.SetBool("isReload", true);
        yield return new WaitForSeconds(t);
        myStats.myGun.curAmmo = myStats.myGun.maxAmmo;
        myStats.myGun.isReload = false;
        myStats.isMove = true;
        myStats.animtor.SetBool("isReload", false);
    }

    public void Hit(float damage)
    {
        Debug.Log("Hit ! : " + myStats.curHP);
        SoundPlayer.instance.PlaySound("Bullet Hitting");
        PhotonNetwork.Instantiate("Blood", transform.position, Quaternion.identity);
        myStats.curHP -= damage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(myStats.myGun.isFire);
            stream.SendNext(myStats.isRunning);
            stream.SendNext(myStats.isJump);
            stream.SendNext(myStats.myGun.isFire);
            stream.SendNext(myStats.myGun.isReload);
            stream.SendNext((int)myStats.isKind);
        }
        else
        {
            myStats.myGun.isFire = (bool)stream.ReceiveNext();
            curMove = (bool)stream.ReceiveNext();
            curJump = (bool)stream.ReceiveNext();
            curFire = (bool)stream.ReceiveNext();
            curReload = (bool)stream.ReceiveNext();
            curKind = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void AddPlayer()
    {
        Main.instance.userList.Add(GetComponent<GamePlayer>());
    }

    [PunRPC]
    void DeadGhost()
    {
        Main.instance.gameState = Main.State.HunterWin;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Interaction");
        if (objs != null)
        {
            foreach (var obj in objs)
            {
                Destroy(obj);
            }
        }
    }
}
