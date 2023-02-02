using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelConfigView : MonoBehaviour
{
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image icon;
    public void Show(LevelConfig config, Action onClick = null)
    {
        trigger.ClearAllEvents();
        trigger.AddEvent(EventTriggerType.PointerClick, eventData => onClick?.Invoke());
        nameText.text = config.levelName;
        icon.sprite = config.icon == null ? ResourceLoader.GetDefaultIcon() : config.icon;
    }
}
