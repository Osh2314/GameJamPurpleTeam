using System.Collections;
using System.Collections.Generic;
using Manager.Pooling;
using GhoseHouse.Object.Hunter;
using UnityEngine;

namespace GhoseHouse.Scenes
{
    public class InGame : MonoBehaviour
    {
        ObjectPool<Hunter> hunterPool = new ObjectPool<Hunter>();
        public GameObject hunterObj = null; 

        void Start()
        {
            hunterPool.Init(hunterObj, 3, null, null, GameObject.Find("Objects").transform);
            hunterPool.Spawn();
        }
    }
}
