using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelConfigView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image icon;
    public void Show(LevelConfig config, Action onClick = null)
    {
        button.OnClick(() => onClick?.Invoke());
        nameText.text = config.levelName;
        icon.sprite = config.icon == null ? ResourceLoader.GetDefaultIcon() : config.icon;
    }
}
