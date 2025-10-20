using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Localization;

public class W3_UIManager : MonoBehaviour
{
    [SerializeField] private W3_GameManager _gameManager;

    public GameObject timer;
    public TextMeshProUGUI _txtLevelTimer;
    public TextMeshProUGUI _txtAttempt;
    public TextMeshProUGUI levelText;
    public Animator levelTextAnimator;
    public Image feedback;
    public Sprite yes, no;
    public float feedbackTime;

    void Update()
    {
        if (_gameManager._levelTimerStatus)
            _txtLevelTimer.text = $"{_gameManager._levelTime:0}";

        else
            _txtLevelTimer.text = " ";
    }

    public void UpdateAttemptTxt()
    {
        _txtAttempt.text = _gameManager.timesPlayed.ToString() + " / 5";
    }

    public void ShowFeedback(bool isCorrect)
    {
        if (isCorrect)
            feedback.sprite = yes;

        else
            feedback.sprite = no;

        StopCoroutine(DisplayFeedback());
        StartCoroutine(DisplayFeedback());
    }

    public void ChangeTimerVisibility(bool state)
    {
        // timer.SetActive(state);
        _gameManager._levelTimerStatus = state;
    }

    public void SetLeveLText(int level)
    {
        levelText.text = $"{LeanLocalization.GetTranslationText("Level")} " + level;
    }

    public void LevelAnimation(bool isLevelUp)
    {
        if (!isLevelUp)
        {
            levelText.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        levelTextAnimator.SetTrigger("LevelAnim");
    }

    IEnumerator DisplayFeedback()
    {
        feedback.DOKill(true);
        feedback.transform.localScale = Vector3.one / 3f;
        feedback.enabled = true;

        feedback.transform.DOScale(Vector3.one, feedbackTime / 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(feedbackTime);

        feedback.enabled = false;
    }
}