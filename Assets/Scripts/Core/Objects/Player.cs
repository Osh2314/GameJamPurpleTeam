using System.Collections;
using System.Collections.Generic;
using Manager.Data;
using UnityEngine;

namespace GhoseHouse.Object
{
    public class Player : MonoBehaviour
    {
        public static Player instance = null;

        [System.Serializable]
        public class Data
        {
            public string Name             = "Ghost";
            public int    Score            = 0;
        }

        [HideInInspector] public Data playerData = new Data();

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            instance = GetComponent<Player>();

            Load();
            Save();
        }

        void Save()
        {
            playerData.Name = DataController.Instance.gameData.Name;
            playerData.Score = DataController.Instance.gameData.Score;
            DataController.Instance.SaveGameData();
        }

        void Load()
        {
            DataController.Instance.LoadGameData();
            playerData = DataController.Instance.gameData;
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Save();
            }
        }
    }
}
