using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        Rect rect = cam.rect;
        float scaleH = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float scaleW = 1f / scaleH;
        if (scaleH < 1)
        {
            rect.height = scaleH;
            rect.y = (1f - scaleH) / 2f;
        }
        else
        {
            rect.width = scaleW;
            rect.x = (1f - scaleW) / 2f;
        }
        cam.rect = rect;
    }

}
