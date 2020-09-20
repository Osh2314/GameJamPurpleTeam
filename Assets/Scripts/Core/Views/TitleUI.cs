using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.View;
using Manager.Sound;

public class TitleUI : BaseScreen<TitleUI>
{
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public override void ShowScreen()
    {
        SoundPlayer.instance.StopBGM();
        SoundPlayer.instance.PlayBGM("Title,LobbyBGM");
        base.ShowScreen();
    }
}
