using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Tools;
public class MusicObject : MonoBehaviour
{
    [HideInInspector] public long timeKey;
    [SerializeField] private List<ParticleSystem> destroyParticle;
    [SerializeField] private List<Rigidbody> postDestroyObjs;
    [SerializeField] private List<Transform> toDestroyObjs;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Collider collider;
    // [SerializeField] private AudioClip destroyClip;
    private bool isDead = false;
    private Vector3 rotationDelta;
    private float minDelta => -10 * Time.deltaTime;
    private float maxDelta => -60 * Time.deltaTime;
    private void Start()
    {
        rotationDelta = new Vector3(RandDelta(), RandDelta(), RandDelta());
        float RandDelta() => UnityEngine.Random.Range(minDelta, maxDelta);
    }
    private void FixedUpdate()
    {
        Rotation();
    }
    private void Rotation()
    {
        if (!isDead) transform.Rotate(rotationDelta);
    }
    public void OnCatch(Action<BaseEventData> callBack)
    {
        trigger.AddEvent(EventTriggerType.PointerEnter, callBack);
    }
    public void Catch()
    {
        if (isDead) return;
        // this.PlayAudio(destroyClip);
        collider.enabled = false;
        trigger?.ClearAllEvents();
        destroyParticle.ForEach(p => p?.Play());
        toDestroyObjs.ForEach(o =>
        {
            o.SetActive(false);
            // rb.AddForce(((AxisEventData)eventData).moveVector.ToVector3());
        });
        postDestroyObjs.ForEach(o =>
        {
            o.SetActive(true);
        });
        if (gameObject) Destroy(gameObject, 10);
        isDead = true;

    }
}
