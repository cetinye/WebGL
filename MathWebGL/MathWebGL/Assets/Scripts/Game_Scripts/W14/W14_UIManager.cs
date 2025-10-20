using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Witmina_Math;

public class W14_UIManager : MonoBehaviour
{
    // FIELD
    [SerializeField] private W14_GameManager _gameManager;

    public Sprite[] resultSprites = new Sprite[2];
    public Button submitBtn;
    public List<GameObject> ticks = new List<GameObject>();
    public TextMeshProUGUI levelTimeText;
    public GameObject operationPanel;
    public Image resultImg;
    public TextMeshProUGUI txtNumber, txtNumber2, txtResult, txtOpSign;
    public TMP_Text levelText;
    public Animator levelAnim;
    public Color numberColor, resultColor;
    public bool isGeneralTimerActive = false;
    public ParticleSystem confetti;

    // MAIN
    void Update()
    {
        PrintTimerText(_gameManager.generalTime, _gameManager.levelTime);
    }

    // FUNC
    /// <summary>
    /// Sets operation sign image from the sign list by the operation.
    /// </summary>
    /// <param name="operation"></param>
    public void SetOperationSign(char operation)
    {
        txtOpSign.text = operation.ToString();
        txtOpSign.gameObject.SetActive(true);
    }

    public void PrintTimerText(float generalTime, float levelTime)
    {
        // Print Level Time
        int lvlMinutes = Mathf.FloorToInt(levelTime / 60);
        int lvlSeconds = Mathf.FloorToInt(levelTime % 60);
        //txtLevelTime.text = $"{lvlMinutes:00}:{lvlSeconds:00}";
        levelTimeText.text = levelTime.ToString("F0");
    }

    public void BlastConfetti()
    {
        confetti.Play();
        W14_AudioManager.instance.PlayOneShot("Confetti");
    }

    public void ShowResult(bool isCorrect)
    {
        if (!isCorrect)
        {
            resultImg.sprite = resultSprites[0];
        }
        else
        {
            resultImg.sprite = resultSprites[1];
        }
        resultImg.gameObject.SetActive(true);
    }

    public void StartGameTimer()
    {
        if (!isGeneralTimerActive)
        {
            isGeneralTimerActive = true;
            _gameManager.generalTime = 60f;
            StartCoroutine(GameTimerRoutine());
        }
    }

    public void LevelAnimation(bool isLevelUp)
    {
        if (!isLevelUp)
        {
            levelText.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        levelAnim.SetTrigger("LevelAnim");
    }

    public void UpdateLevelText(int _level)
    {
        levelText.text = LeanLocalization.GetTranslationText("Level") + " " + _level;
    }

    IEnumerator GameTimerRoutine()
    {
        for (int i = 0; i < ticks.Count; i++)
        {
            yield return new WaitForSeconds(4f);
            ticks[i].SetActive(false);
        }
    }
}