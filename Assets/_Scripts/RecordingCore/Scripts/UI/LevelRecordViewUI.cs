using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelRecordViewUI : WindowBase
{
    public Button recordButton;
    public void Show(AudioClip clip, Action recordClick)
    {
        recordButton.OnClick(() =>
        {
            recordClick.Invoke();
        });
    }
}
