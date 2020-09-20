using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Manager.Sound;

public class Main : MonoBehaviourPunCallbacks
{
    public static Main instance = null;

    public enum State
    {
        GhostWin, HunterWin, NONE = 99
    }

    #region [ Title ]
    [SerializeField] private Text lobbyInfoText = null;
    [SerializeField] private Text roomNameText = null;
    [SerializeField] private Text roomInfoText = null;
    [SerializeField] private Text playerlistText = null;
    [SerializeField] private InputField nameInput = null;
    [SerializeField] private InputField roomInput = null;

    [SerializeField] private Button[] cellButton = null;
    [SerializeField] private Button PreviousButton = null;
    [SerializeField] private Button NextButton = null;

    [SerializeField] private Button GameStartButoon = null;

    List<RoomInfo> myList = new List<RoomInfo>();
    int curPage = 1, maxPage = 0, multiple = 0;
    public PhotonView PV;
    #endregion

    #region [ Game ]
    public List<GamePlayer> userList = new List<GamePlayer>();
    public GameObject winnerObj = null;
    public State gameState = State.NONE;
    [SerializeField] private Text RPCmsg = null;
    [SerializeField] private GameObject clockObj = null;
    [SerializeField] private GameObject hunterSpawnPos = null;
    [SerializeField] private GameObject ghostSpawnPos = null;
    [SerializeField] private GameObject[] goalSpawnPos = null;

    public int goalCost = 0;
    public bool LightBroken = false;
    #endregion

    void Awake()
    {
        instance = GetComponent<Main>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        SoundPlayer.instance.StopBGM();
        SoundPlayer.instance.PlayBGM("Title,LobbyBGM");
    }

    void Update()
    {
        lobbyInfoText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다 ! " + PhotonNetwork.CountOfPlayers + "명 접속중";

        if (gameState == State.GhostWin)
        {
            // 게임종료
            PV.RPC("SendMsg", RpcTarget.AllBuffered, "유령 승리 !");
            Invoke("OnEndGame", 5f);
        }
        else if (gameState == State.HunterWin)
        {
            // 게임종료
            PV.RPC("SendMsg", RpcTarget.AllBuffered, "헌터 승리 !");
            Invoke("OnEndGame", 5f);
        }
    }

    public void OnConnect() => PhotonNetwork.ConnectUsingSettings();

    public void OnDisconnect() => PhotonNetwork.Disconnect();

    public void OnJoinLobby() => PhotonNetwork.JoinLobby();

    public void OnCreateRoom() => PhotonNetwork.CreateRoom(roomInput.text == "" ? roomInput.text = "유령퇴치 " + PhotonNetwork.CountOfRooms : roomInput.text, new RoomOptions { MaxPlayers = 5 });

    public void OnRandomJoinRoom() => PhotonNetwork.JoinRandomRoom();

    public void OnJoinRoom() => PhotonNetwork.JoinRoom(roomInput.text);

    public void OnLeaveRoom()
    {
        userList.Clear();
        gameState = State.NONE;
        GameStartButoon.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect Server");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        TitleUI.instance.ShowScreen();
        LobbyUI.instance.HideScreen();
        RoomUI.instance.HideScreen();
        GameStartButoon.interactable = false;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        TitleUI.instance.HideScreen();
        RoomUI.instance.HideScreen();
        LobbyUI.instance.ShowScreen();

        PhotonNetwork.LocalPlayer.NickName = nameInput.text;
        myList.Clear();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room");
        GameStartButoon.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MainCamera");
        if (objs.Length > 1)
        {
            foreach (var obj in objs)
            {
                if (obj != objs[0])
                {
                    Destroy(obj);
                }
            }
        }

        TitleUI.instance.HideScreen();
        LobbyUI.instance.HideScreen();
        RoomUI.instance.ShowScreen();
        PhotonNetwork.Instantiate("Player", hunterSpawnPos.transform.position, Quaternion.identity);
        RoomRenewal();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed");
        roomInput.text = "";
        OnCreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
    }

    void RoomRenewal()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        roomInput.text = "";
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomInfoText.text = "[ " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " ]";
        playerlistText.text = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.PlayerList[0])
            {
                playerlistText.text += "[Admin] " + player.NickName + "\n";
            }
            else
            {
                playerlistText.text += player.NickName + "\n";
            }
        }
        foreach (var user in userList)
        {
            if (user == null)
                userList.Remove(user);
        }
    }

    public void MyListClick(int num)
    {
        if (num == -2) --curPage;
        else if (num == -1) ++curPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % cellButton.Length == 0) ? myList.Count / cellButton.Length : myList.Count / cellButton.Length + 1;

        PreviousButton.interactable = (curPage <= 1) ? false : true;
        NextButton.interactable = (curPage >= maxPage) ? false : true;

        multiple = (curPage - 1) * cellButton.Length;
        for (int i = 0; i < cellButton.Length; i++)
        {
            cellButton[i].interactable = (multiple + i < myList.Count) ? true : false;
            cellButton[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            cellButton[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }

    public void OnStart()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            PV.RPC("GoGame", RpcTarget.All);
        }
    }

    public void OnEndGame()
    {
        ReSetGame();
    }

    [PunRPC]
    void LightB()
    {
        LightBroken = true;
        Invoke("SkillOff", 10f);
    }

    void SkillOff()
    {
        LightBroken = false;
    }

    [PunRPC]
    void GoGame()
    {
        SoundPlayer.instance.StopBGM();
        SoundPlayer.instance.PlayBGM("InGameBGM(NoBattle)");

        goalCost = 0;
        winnerObj.SetActive(false);

        RoomUI.instance.HideScreen();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameState = State.NONE;

        userList[0].myStats.isKind = GamePlayer.Kind.Ghost;
        // Set Ghost
        foreach (var user in userList)
        {
            user.myStats.isGame = true;
            user.myStats.isMove = true;

            if (user.myStats.PV.IsMine)
            {
                if (user.myStats.isKind == GamePlayer.Kind.Ghost)
                {
                    user.gameObject.transform.position = ghostSpawnPos.transform.position;
                    user.myStats.myCharacter = user.gameObject.transform.GetChild(4).gameObject;
                    user.myStats.animtor = null;
                    user.myStats.ghostLight.SetActive(true);

                    for (int i = 0; i < userList.Count - 1; i++)
                        Instantiate(clockObj, goalSpawnPos[Random.Range(i, i + 1)].transform.position, Quaternion.identity);

                    //user.myStats.curHP = user.myStats.maxHP = 25f * (userList.Count - 1);
                }
                user.myStats.myCharacter.SetActive(true);
            }
            else if (!user.myStats.PV.IsMine && user.myStats.isKind == GamePlayer.Kind.Ghost)
            {
                user.gameObject.transform.position = ghostSpawnPos.transform.position;
                user.myStats.myCharacter = user.gameObject.transform.GetChild(4).gameObject;
                user.myStats.animtor = null;
            }
            else if (!user.myStats.PV.IsMine && user.myStats.isKind != GamePlayer.Kind.Ghost)
            {
                user.myStats.setChar = true;
            }
            user.myStats.myCharacter.SetActive(true);
        }
    }

    [PunRPC]
    void SetWinner(State state)
    {
        gameState = state;
    }

    [PunRPC]
    void ReSetGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        OnDisconnect();
        userList.Clear();
        gameState = State.NONE;

        GameStartButoon.interactable = false;

        foreach (var user in userList)
        {
            Destroy(user.gameObject);
        }
    }

    [PunRPC]
    public void SendMsg(string text = "")
    {
        RPCmsg.text = text;
        RPCmsg.transform.gameObject.SetActive(true);
    }

    [ContextMenu("Test Msg")]
    void TestMsg()
    {
        PV.RPC("SendMsg", RpcTarget.AllBuffered, "지정된 목표를 달성하세요 !");
    }

    public void OnExit()
    {
        Application.Quit(0);
    }
}