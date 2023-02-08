using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

public class LosingGameScreen : WindowBase
{
    public Image filled;
    public Button acceptBtn;
    private Coroutine timerCoroutine;
    public void Show(Action onContinue, Action onTimerEnd)
    {
        acceptBtn.OnClick(() =>
        {
            onContinue.Invoke();
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            Close();
        });
        timerCoroutine = StartCoroutine(Timer(5, onTimerEnd));
    }
    private IEnumerator Timer(int secondsCount, Action onTimerEnd)
    {
        var wait = new WaitForFixedUpdate(); 
        filled.fillAmount = 1;
        while (filled.fillAmount > 0)
        {
            filled.fillAmount -= Time.fixedDeltaTime / secondsCount;
            yield return wait;
        }
        onTimerEnd.Invoke();
    }
}
