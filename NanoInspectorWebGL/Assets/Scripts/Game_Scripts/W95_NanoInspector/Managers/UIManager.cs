using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace NanoInspector
{
    public class UIManager : MonoBehaviour
    {
        public float questionTimer;
        public float gameTimer;
        public TextMeshProUGUI questionText;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Image rightMonitorScreen, leftMonitorScreen;
        [SerializeField] private RectTransform timerScreenRect;
        [SerializeField] private RectTransform statScreenRect;
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TextMeshProUGUI gameTimerText;
        [SerializeField] private TextMeshProUGUI correctText;
        [SerializeField] private TextMeshProUGUI wrongText;
        [SerializeField] private Image questionTimerFill;
        [SerializeField] private Image correctWrongLight;
        [SerializeField] private Sprite correctLight;
        [SerializeField] private Sprite wrongLight;
        [SerializeField] private Image lightbulb;
        [SerializeField] private Sprite defaultLightbulb;
        [SerializeField] private Sprite correctLightbulb;
        [SerializeField] private Sprite wrongLightbulb;

        [SerializeField] private Vector2 r_monitorEndPos;
        [SerializeField] private Vector2 l_monitorEndPos;
        [SerializeField] private Vector2 r_monitorStartPos;
        [SerializeField] private Vector2 l_monitorStartPos;
        [SerializeField] private Vector2 questionPanelStartPos;
        [SerializeField] private Vector2 questionPanelEndPos;
        [SerializeField] private Vector2 timerScreenStartPos;
        [SerializeField] private Vector2 timerScreenEndPos;
        [SerializeField] private Vector2 statScreenStartPos;
        [SerializeField] private Vector2 statScreenEndPos;

        [SerializeField] private float flashInterval = 0.5f;
        [SerializeField] private bool isFlashable = true;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            // gameTimer = WManagers.WRCM.GetGamePlaytime();
            gameTimer = 90;
        }

        private void Update()
        {
            GameTimer();
            QuestionTimer();
        }

        public void SetVariables()
        {
            questionTimer = levelManager.question.questionTimer;
        }

        private void GameTimer()
        {
            if (GameManager.instance.state == GameManager.GameState.Playing)
            {
                //timer continue if game is playing
                if (gameTimer > 0)
                {
                    gameTimer -= Time.deltaTime;
                    gameTimerText.text = gameTimer.ToString("00");
                }
                //stop timer if time ran out
                else if (gameTimer <= 0)
                {
                    gameTimer = 0;
                    GameManager.instance.state = GameManager.GameState.Idle;
                    levelManager.EndLevel();
                }

                if (gameTimer <= 5.2f && isFlashable)
                {
                    isFlashable = false;
                    // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                    FlashRed();
                }
            }
        }

        private void QuestionTimer()
        {
            if (levelManager.isQuestionTimerOn)
            {
                //timer continue if game is playing
                if (questionTimer > 0)
                {
                    questionTimer -= Time.deltaTime;
                    questionTimerFill.fillAmount = questionTimer / levelManager.question.questionTimer;
                }
                //stop timer if time ran out
                else if (questionTimer < 0)
                {
                    levelManager.isButtonPressable = false;
                    levelManager.isQuestionTimerOn = false;
                    questionTimer = 0;
                    levelManager.IncreaseWrongCount();
                    levelManager.StartLevel();
                }
            }
        }

        public void SetStats(int correct, int wrong)
        {
            correctText.text = correct.ToString();
            wrongText.text = wrong.ToString();
        }

        public void SetQuestionPanel(bool state)
        {
            questionPanel.SetActive(state);
        }

        public void LightGreen()
        {
            StopAllCoroutines();
            StartCoroutine(LightGreenRoutine());
        }

        public void LightRed()
        {
            StopAllCoroutines();
            StartCoroutine(LightRedRoutine());
        }

        public void StartAnimation()
        {
            StartCoroutine(StartAnimationRoutine());
        }

        private void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(gameTimerText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(gameTimerText.DOColor(Color.black, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }

        IEnumerator LightGreenRoutine()
        {
            correctWrongLight.sprite = correctLight;
            correctWrongLight.enabled = true;

            lightbulb.sprite = correctLightbulb;

            yield return new WaitForSeconds(0.3f);

            correctWrongLight.enabled = false;

            lightbulb.sprite = defaultLightbulb;
        }

        IEnumerator LightRedRoutine()
        {
            correctWrongLight.sprite = wrongLight;
            correctWrongLight.enabled = true;

            lightbulb.sprite = wrongLightbulb;

            yield return new WaitForSeconds(0.3f);

            correctWrongLight.enabled = false;

            lightbulb.sprite = defaultLightbulb;
        }

        IEnumerator StartAnimationRoutine()
        {
            AudioManager.instance.Play("Background");

            rightMonitorScreen.rectTransform.anchoredPosition = r_monitorStartPos;
            leftMonitorScreen.rectTransform.anchoredPosition = l_monitorStartPos;
            timerScreenRect.anchoredPosition = timerScreenStartPos;
            statScreenRect.anchoredPosition = statScreenStartPos;

            RectTransform questionRect = questionPanel.GetComponent<RectTransform>();
            questionRect.anchoredPosition = questionPanelStartPos;

            AudioManager.instance.PlayOneShot("ScreenPanel");
            Tween monitorMove = rightMonitorScreen.rectTransform.DOAnchorPos(r_monitorEndPos, 1f);
            leftMonitorScreen.rectTransform.DOAnchorPos(l_monitorEndPos, 1f);
            timerScreenRect.DOAnchorPos(timerScreenEndPos, 1f);
            statScreenRect.DOAnchorPos(statScreenEndPos, 1f);
            yield return monitorMove.WaitForCompletion();

            AudioManager.instance.PlayOneShot("QuestionPanel");
            Tween questionMove = questionRect.DOAnchorPos(questionPanelEndPos, 0.8f);
            yield return questionMove.WaitForCompletion();

            GameManager.instance.state = GameManager.GameState.Playing;
            levelManager.StartLevel();
        }
    }
}