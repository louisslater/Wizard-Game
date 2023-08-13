using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private float fadeInSpeed;
    private float fadeOutSpeed;
    bool fadeIn = false;
    bool fadeOut = false;
    bool inventoryOpen = false;

    public void ShowUI(float fadeVal)
    {
        //Fades in the UI
        fadeIn = true;
        fadeInSpeed = fadeVal;
        inventoryOpen = true;
    }

    public void HideUI(float fadeVal)
    {
        //Fades out the UI
        fadeOut = true;
        fadeOutSpeed = fadeVal;
        inventoryOpen = false;
    }

    public void ShowThenHideUI(float sec, float fadeInVal, float fadeOutVal)
    {
        //Fade in the UI, then fades it out after a set time. Function is disabled if the inventory is open (so the items don't fade away).
        if (!inventoryOpen)
        {
            fadeIn = true;
            fadeOut = false;
            fadeInSpeed = fadeInVal;
            fadeOutSpeed = fadeOutVal;
            StartCoroutine(Waiting(sec));
        }
    }

    IEnumerator Waiting(float sec)
    {
        // Waits for "sec" seconds
        yield return new WaitForSeconds(sec);
        if (!inventoryOpen)
        {
            fadeOut = true;
            fadeIn = false;
        }
    }

    private void Update()
    {
        // If fadeOut is true, the UI will fade out. if fadeIn is true the UI will pop back on
        if (fadeOut)
        {
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeOutSpeed;
                if (canvasGroup.alpha == 0 || fadeIn)
                {
                    fadeOut = false;
                }
            }
        }
        else if (fadeIn)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * fadeInSpeed;
                if (canvasGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }
    }
}
