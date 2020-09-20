using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Msg : MonoBehaviourPunCallbacks
{
    public static Msg instance = null;
    [SerializeField] private Text myText = null;

    void Awake()
    {
        instance = GetComponent<Msg>();
    }

#pragma warning disable CS0114 
    private void OnEnable()
#pragma warning restore CS0114 
    {
        StartCoroutine(FadeIn(2.5f));
    }

    IEnumerator FadeIn(float t = 3f)
    {
        float progress = 0f;
        while (progress <= t)
        {
            progress += Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, progress * 1f);
            yield return null;
        }
        StartCoroutine(FadeOut(2.5f));
    }

    IEnumerator FadeOut(float t = 3f)
    {
        float progress = t;
        while (progress > 0)
        {
            progress -= Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, progress * 1f);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
