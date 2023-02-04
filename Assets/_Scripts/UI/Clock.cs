using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Game.UI;

public class Clock : BaseAnimationBG
{
    [SerializeField] private RectTransform bg, secondsArrow, minuteArrow;
    private long endTime;
    private float stepAngle => 360 / 12;
    private Coroutine stepsCoroutune;
    private void Start()
    {
        stepsCoroutune = StartCoroutine(StepsCoroutine());
    }
    private IEnumerator StepsCoroutine()
    {
        var waitFor = new WaitForSeconds(1);
        while (true)
        {
            yield return waitFor;
            secondsArrow.DORotate(new Vector3(0, 0, secondsArrow.rotation.eulerAngles.z - stepAngle), 0.5f);
            minuteArrow.DORotate(new Vector3(0, 0, minuteArrow.rotation.eulerAngles.z - (stepAngle / 12)), 0.5f);
        }
    }
    public override void StartDanceAnimation(float animationDuration)
    {
        bg.DOScale(new Vector3(1.1f, 1.1f, 1.1f), animationDuration * 0.2f).OnComplete(() =>
        {
            bg.DOScale(new Vector3(1, 1, 1), animationDuration * 0.8f);
        });
    }
}