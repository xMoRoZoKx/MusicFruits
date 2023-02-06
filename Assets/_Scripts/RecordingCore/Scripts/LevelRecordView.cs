using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tools;
using Game.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class LevelRecordView : MonoBehaviour
{
    public EventTrigger recordEventTrigger;
    public Button save, stop;
    public Slider slider;
    private AudioSource source;
    private LevelRecorder recorder = new LevelRecorder();
    [SerializeField] private LevelConfig config;
    [SerializeField] private MultiButton xButton;
    private void Awake()
    {
        slider.maxValue = config.clip.length;
        save.OnClick(() => recorder.EndRecord());
        stop.OnClick(() => recorder.Pause(true));

        xButton.AddAction("0.5x", () =>
        {
            recorder.SetSpeed(0.5f);
        });
        xButton.AddAction("0.75x", () =>
        {
            recorder.SetSpeed(0.75f);
        });
        xButton.AddAction("1x", () =>
        {
            recorder.SetSpeed(1f);
        });
        // xButton.AddAction("1.5x", () =>
        // {
        //     recorder.SetSpeed(1.5f);
        // });
        xButton.Set(0);


        recordEventTrigger.AddEvent(EventTriggerType.PointerDown, eventData =>
        {
            if (!recorder.RecordStarted)
            {
                source = this.GetOrAddCommponent<AudioSource>();
                recorder.StartRecord(source, config, onEnd: timeKeys =>
                {
                    config.timeKeysJson = JsonUtility.ToJson(timeKeys);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(config);
#endif
                });
            }
            else recorder.RecordPoint();
        });
        slider.onValueChanged.AddListener(value =>
        {
            if (!recorder.IsPaused) return;
            recorder.SetTime((long)value.SecondsToMilliseconds());
        });
        // WindowManager.instance.Show<LevelRecordViewUI>().Show(clip, () => recorder.RecordPoint());
    }
    private void FixedUpdate()
    {
        if (source == null || recorder.IsPaused) return;
        slider.value = source.time;
    }
}
