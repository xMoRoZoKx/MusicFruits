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
    [SerializeField] private Rigidbody rb;
    private bool isDead = false;
    public void OnCatch(Action<BaseEventData> callBack)
    {
        trigger.AddEvent(EventTriggerType.PointerEnter, callBack);
    }
    public void Catch(BaseEventData eventData)
    {
        if (isDead) return;
        trigger?.ClearAllEvents();
        destroyParticle.ForEach(p => p?.Play());
        toDestroyObjs.ForEach(o =>
        {
            o.SetActive(false);
            rb.AddForce(((AxisEventData)eventData).moveVector.ToVector3());
        });
        postDestroyObjs.ForEach(o =>
        {
            o.SetActive(true);
        });
        if (gameObject) Destroy(gameObject, 10);
        isDead = true;
        
    }
}
