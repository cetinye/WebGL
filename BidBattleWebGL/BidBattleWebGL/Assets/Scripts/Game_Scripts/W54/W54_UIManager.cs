using System.Collections;
using DG.Tweening;
using Unity_CS;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using W54_Managers;
using W54;

public class W54_UIManager : MonoBehaviour
{
    [SerializeField] private W54_GameManager gameManager;
    [SerializeField] private CanvasGroup questionPanel;
    [SerializeField] private Image artPieceContainer;
    [SerializeField] private Sprite[] artPieces;
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private Animator character;
    [SerializeField] private Image[] questionContainers;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Color[] containerColors;
    
    [SerializeField] private Animator leftCurtain;
    [SerializeField] private Animator rightCurtain;

    //Countdown
    [SerializeField] private float countdownTime = 5f;
    [SerializeField] private TMP_Text countdownText;

    private bool isCountdownOn = false;
    private float countdownTimer = 5f;

    private void Update()
    {
        Countdown();
    }

    public void QuestionAnswered(int buttonIndex, bool isCorrect)
    {
        ControlAnswerButtonInteractivity(false);
        StartCoroutine(ChangeContainerColors(buttonIndex, isCorrect));
        ChangeArtPiece();
        MoveArm();
        AudioManager.instance.PlayOneShot(SoundType.W54_QuestionAnswered);
    }

    private IEnumerator ChangeContainerColors(int buttonIndex, bool isCorrect)
    {
        var resultColor = isCorrect ? containerColors[1] : containerColors[2];
        
        if (countdownTimer > 0)
            questionContainers[buttonIndex].color = resultColor;

        else
        {
            questionContainers[0].color = containerColors[2];
            questionContainers[1].color = containerColors[2];
        }

        yield return new WaitForSeconds(1);
        ResetContainerColors();
    }

    private void ResetContainerColors()
    {
        questionContainers[0].color = containerColors[0];
        questionContainers[1].color = containerColors[0];
    }

    private void MoveArm()
    {
        character.Play("Move", 0, 0f);
        Invoke("PlaySparks", 0.5f);
    }

    private void PlaySparks()
    {
        sparks.Play();
        AudioManager.instance.PlayOneShot(SoundType.W54_HitToPad);
        Taptic.Medium();
    }

    private void ChangeArtPiece()
    {
        confetti.Play();
        artPieceContainer.sprite = artPieces._RandomItem();
    }

    public void ControlQuestionPanelVisibility(bool isActive)
    {
        var alpha = isActive ? 1f : 0f;
        var tweenDuration = 0.75f;
        questionPanel.DOFade(alpha, tweenDuration);
    }

    public void ControlAnswerButtonInteractivity(bool isActive)
    {
        foreach (var button in answerButtons)
        {
            button.interactable = isActive;
        }
    }
    
    public void OpenCurtains(TweenCallback callback)
    {
        leftCurtain.enabled = true;
        rightCurtain.enabled = true;
        leftCurtain.Play("Move", 0, 0);
        rightCurtain.Play("Move", 0, 0);
        AudioManager.instance.PlayOneShot(SoundType.W54_CurtainMove);

       var tween = leftCurtain.transform.DOLocalMoveX(-1000, 1.1f).OnComplete(() => leftCurtain.enabled = false);
        rightCurtain.transform.DOLocalMoveX(1000, 1.1f).OnComplete(() => rightCurtain.enabled = false);

        tween.OnComplete(() => callback.Invoke());
    }
    public void CloseCurtains(TweenCallback callback)
    {
        leftCurtain.enabled = true;
        rightCurtain.enabled = true;
        leftCurtain.Play("Move", 0, 0);
        rightCurtain.Play("Move", 0, 0);
        AudioManager.instance.PlayOneShot(SoundType.W54_CurtainMove);

        var tween = leftCurtain.transform.DOLocalMoveX(0, 1.5f).OnComplete(() => leftCurtain.enabled = false);
        rightCurtain.transform.DOLocalMoveX(0, 1.5f).OnComplete(() => rightCurtain.enabled = false);
        
        tween.OnComplete(() => callback.Invoke());
    }

    private void Countdown()
    {
        if (isCountdownOn)
        {
            //timer continue if game is playing
            if (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                countdownText.text = countdownTimer.ToString("0");
            }
            //stop timer if time ran out
            else if (countdownTimer < 0)
            {
                isCountdownOn = false;
                countdownTimer = 0;
                gameManager.QuestionAnswered(0);
            }
        }
    }

    public void ResetCountdown()
    {
        countdownTimer = countdownTime;
    }

    public void SetCountdown(bool state)
    {
        countdownText.enabled = state;
        isCountdownOn = state;
    }

    public float GetCountdown()
    {
        return countdownTimer;
    }
}