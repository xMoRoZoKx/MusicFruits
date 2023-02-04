using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using Tools.PlayerPrefs;
using UnityEngine;
public class TimeKeys
{
    public List<long> timeKeysMilliseconds = new List<long>();
}
public class LevelRecorder
{
    public const string SAVE_KEY = "SAVE_LEVEL_KEY";
    public List<long> timeKeys { private set; get; }
    private Action<TimeKeys> onRecordEnd;
    private float speed = 1;
    public bool RecordStarted => source != null;
    public bool IsPaused => source == null || !source.isPlaying;
    AudioSource source;

    private long GetCurrentTimeKeyMilliseconds()
    {
        return (long)(source.time.SecondsToMilliseconds());
    }
    public void StartRecord(AudioSource audioSource, AudioClip clip, Action<TimeKeys> onEnd = null)
    {
        timeKeys = new List<long>();
        source = audioSource;
        source.clip = clip;
        source.pitch = speed;
        audioSource.Play();
        onRecordEnd = onEnd;

        RecordPoint();
    }
    public void SetTime(long time)
    {
        if (!RecordStarted)
        {
            Debug.LogError("record not started!");
            return;
        }
        source.time = time.MillisecondsToSeconds();
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
        if (!RecordStarted) return;
        source.pitch = speed;
    }
    public void Pause(bool status)
    {
        if (!RecordStarted)
        {
            Debug.LogError("record not started!");
            return;
        }
        if (status)
            source.Pause();
        else if (source != null && !source.isPlaying)
        {
            var time = GetCurrentTimeKeyMilliseconds();
            timeKeys.RemoveAll(tk => tk > time);
            source.PlayScheduled(time.MillisecondsToSeconds());
        }

    }
    public void RecordPoint()
    {
        if (!RecordStarted)
        {
            Debug.LogError("record not started!");
            return;
        }
        Pause(false);
        var currentTime = GetCurrentTimeKeyMilliseconds();
        timeKeys.Add(currentTime);
        Debug.Log(currentTime);
        if (currentTime >= source.clip.length.SecondsToMilliseconds())
            EndRecord();
    }
    public void EndRecord()
    {
        if (!RecordStarted)
        {
            Debug.LogError("record not started!");
            return;
        }
        RecordPoint();
        source.Stop();
        source = null;
        TimeKeys keys = new TimeKeys();
        keys.timeKeysMilliseconds = timeKeys;
        onRecordEnd?.Invoke(keys);
        PlayerPrefsPro.Set(SAVE_KEY, keys);
        Debug.Log("record completed, you json:\n");
        Debug.Log(JsonUtility.ToJson(keys));
    }
}
