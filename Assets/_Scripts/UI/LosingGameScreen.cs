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
    public void Show(Action onContinue)
    {
        acceptBtn.OnClick(() =>
        {
            onContinue.Invoke();
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            Close();
        });
        timerCoroutine = StartCoroutine(Timer(5));
    }
    private IEnumerator Timer(int secondsCount)
    {
        var wait = new WaitForFixedUpdate();
        while (filled.fillAmount >= 0)
        {
            filled.fillAmount -= Time.deltaTime / secondsCount;
            yield return wait;
        }
        GameSession.Instance.ComliteLevel(false);
    }
}
