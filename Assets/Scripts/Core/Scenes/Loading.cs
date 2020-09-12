using Manager.Scene;
using UnityEngine;

namespace GhoseHouse.Scenes
{
    public class Loading : MonoBehaviour
    {
        void Awake()
        {
            Loader.LoaderCallback();
        }
    }
}