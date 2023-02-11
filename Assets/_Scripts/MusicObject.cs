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
    [SerializeField] private AudioClip destroyClip;
    private bool isDead = false;
    private Vector3 rotationDelta;
    public Action OnDestroyEvent;
    protected virtual float minDelta => -30 * Time.deltaTime;
    protected virtual float maxDelta => -60 * Time.deltaTime;
    private void Start()
    {
        rotationDelta = new Vector3(RandDelta(), RandDelta(), RandDelta());
        float RandDelta() => UnityEngine.Random.Range(minDelta, maxDelta);
        trigger.AddEvent(EventTriggerType.PointerEnter, eventData => Catch());
        ReactiveList<MultiButton> t = new ReactiveList<MultiButton>();
    }
    private void FixedUpdate()
    {
        Rotation();
    }
    private void Rotation()
    {
        if (!isDead) transform.Rotate(rotationDelta);
    }
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
    public void OnCatch(Action callBack)
    {
        trigger.AddEvent(EventTriggerType.PointerEnter, eventData => callBack?.Invoke());
    }
    public void Catch()
    {
        if (isDead) return;
        if(destroyClip != null) this.PlayAudio(destroyClip);
        collider.enabled = false;
        trigger?.ClearAllEvents();
        destroyParticle.ForEach(p => p?.Play());
        toDestroyObjs.ForEach(o =>
        {
            o.SetActive(false);
        });
        postDestroyObjs.ForEach(o =>
        {
            o.SetActive(true);
        });
        if (gameObject) Destroy(gameObject, 10);
        isDead = true;

    }
}
