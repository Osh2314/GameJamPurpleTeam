using System.IO;
using UnityEngine;
using GhoseHouse.Object;

namespace Manager.Data
{
    public class DataController : MonoBehaviour
    {
        static GameObject _container;
        static GameObject Container
        {
            get
            {
                return _container;
            }
        }
        static DataController _instance;
        public static DataController Instance
        {
            get
            {
                if (!_instance)
                {
                    _container = new GameObject();
                    _container.name = "DataController";
                    _instance = _container.AddComponent(typeof(DataController)) as DataController;
                    DontDestroyOnLoad(_container);
                }
                return _instance;
            }
        }
        public string GameDataFileName = "DataInform.json";

        public Player.Data _gameData;
        public Player.Data gameData
        {
            get
            {
                if (_gameData == null)
                {
                    LoadGameData();
                    SaveGameData();
                }
                return _gameData;
            }
        }

        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/" + GameDataFileName;
            Debug.Log(filePath);
            if (File.Exists(filePath))
            {
                string FromJsonData = File.ReadAllText(filePath);
                _gameData = JsonUtility.FromJson<Player.Data>(FromJsonData);
            }
            else
            {
                _gameData = new Player.Data();
            }
        }

        public void SaveGameData()
        {
            string ToJsonData = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + "/" + GameDataFileName;
            File.WriteAllText(filePath, ToJsonData);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGameData();
            }
        }
    }
}