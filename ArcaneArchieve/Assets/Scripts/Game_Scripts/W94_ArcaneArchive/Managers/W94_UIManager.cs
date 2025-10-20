using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using System;
using Arcane_Archive;
using Lean.Localization;

public class W94_UIManager : MonoBehaviour
{
    private int remainingTime;
    [SerializeField] private Image blackScreen;
    [SerializeField] private Image candleoutScreen;
    [SerializeField] private Image frameLower;
    [SerializeField] private Image frameOuter;
    [SerializeField] private Image candle;
    // [SerializeField] private List<Sprite> candleSprites = new List<Sprite>();

    private float time;
    private bool lockFlag = true;
    private int candleIndex = 0;
    private bool isCandleStarted = false;

    private float flashInterval = 0.5f;
    private bool isFlashable = true;
    private bool isCandleoutStarted = false;

    [Header("Intro variables")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip enVideoClip;
    [SerializeField] private VideoClip trVideoClip;
    [SerializeField] private GameObject videoOnCanvas;
    [SerializeField] private GameObject skipButton;
    private int introWatchedBefore;

    // Update is called once per frame
    void Update()
    {
        if (W94_GameManager.instance.state == W94_GameManager.GameState.playing)
            Timer();
    }

    private void Timer()
    {
        time -= Time.deltaTime;

        if (time <= 5.2f && isFlashable)
        {
            isFlashable = false;
            // W94_GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
            FlashRed();
        }

        if (time <= 30f && !isCandleoutStarted)
        {
            isCandleoutStarted = true;
            candleoutScreen.DOFade(0.7f, 30f).SetEase(Ease.Linear);
        }

        if (time < 0 && lockFlag)
        {
            lockFlag = false;
            StartCoroutine(TimesUp());
            CancelInvoke();
        }

        if (!isCandleStarted)
        {
            isCandleStarted = true;
            InvokeRepeating(nameof(UpdateCandleRoutine), 1f / 5f, 1f / 5f);
        }
    }

    private void UpdateCandleRoutine()
    {
        // if (candleSprites[candleIndex] != null)
        // {
        //     candle.sprite = candleSprites[candleIndex];
        //     candleIndex++;
        // }
    }

    IEnumerator TimesUp()
    {
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;

        W94_AudioManager.instance.Stop("Background");
        W94_AudioManager.instance.Play("TimesUp");

        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(1, 2);
        blackScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1, 2);
        yield return new WaitForSeconds(4f);

        W94_GameManager.instance.Finish(false);
    }

    public void DisableBlackScreen()
    {
        blackScreen.DOFade(0, 0.1f);
        blackScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, 0.1f);
        blackScreen.gameObject.SetActive(false);
    }

    public void ResetTime()
    {
        remainingTime = W94_LevelManager.levelSO.totalTime;
        time = remainingTime;
    }

    public void ArrangeFrames(Transform parent)
    {
        frameLower.transform.SetParent(parent);
        frameOuter.transform.SetParent(parent);

        frameLower.transform.SetAsLastSibling();
        frameOuter.transform.SetAsLastSibling();
    }

    private void FlashRed()
    {
        Sequence redFlash = DOTween.Sequence();

        redFlash.Append(candle.DOColor(Color.red, flashInterval))
                .SetEase(Ease.Linear)
                .Append(candle.DOColor(Color.white, flashInterval))
                .SetEase(Ease.Linear)
                .SetLoops(6);

        redFlash.Play();
    }

    #region Intro
    public void StartIntro()
    {
        videoOnCanvas.SetActive(false);
        // videoPlayer.clip = LeanLocalization.Instances[0].CurrentLanguage == "English" ? enVideoClip : trVideoClip;

        // videoPlayer.Play();
        // Invoke("EndReached", (float)videoPlayer.clip.length);
        // Invoke("FadeOut", (float)videoPlayer.clip.length - 3f);
        // introWatchedBefore = PlayerPrefs.GetInt("W94_introWatchedBefore", 0);
        // if (introWatchedBefore == 1)
        //     skipButton.SetActive(true);
    }

    private void EndReached()
    {
        //close the gameobject and stop the video
        videoOnCanvas.SetActive(false);
        PlayerPrefs.SetInt("W94_introWatchedBefore", 1);
        skipButton.SetActive(false);
        videoPlayer.Stop();
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;
        W94_GameManager.instance.StartGame();
    }

    public void SkipIntro()
    {
        CancelInvoke();
        videoPlayer.Stop();
        skipButton.SetActive(false);
        videoOnCanvas.SetActive(false);
        W94_GameManager.instance.state = W94_GameManager.GameState.idle;
        W94_GameManager.instance.StartGame();
    }

    public void PlayPauseVideo()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    public void FadeOut()
    {
        StartCoroutine(LerpFunction(0f, 3f));
    }

    IEnumerator LerpFunction(float endValue, float duration)
    {
        float time = 0;
        float startValue = videoPlayer.GetDirectAudioVolume(0);
        while (time < duration)
        {
            videoPlayer.SetDirectAudioVolume(0, Mathf.Lerp(startValue, endValue, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        videoPlayer.SetDirectAudioVolume(0, endValue);
    }
    #endregion
}
