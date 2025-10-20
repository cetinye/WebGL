using DG.Tweening;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public int numOfAttempts;
        public int moveCounter = 0;
        public bool infoTabletLevel = false;
        public float timeRemaining;
        public bool startTransitionFlag = false;

        [SerializeField] private GameObject GameCanvas;
        [SerializeField] private GameObject LevelSelectionCanvas;
        [SerializeField] private Slider numOfAttemptsSlider;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private Camera animCamera;
        [SerializeField] private Image gameBackground;
        [SerializeField] private GameObject glow;
        [SerializeField] private GameObject glowHalfCircle;
        [SerializeField] private GameObject correctUI;
        [SerializeField] private GameObject wrongUI;
        [SerializeField] private Vector3 feedbackUIScale;
        [SerializeField] private float feedbackUIScaleTime;
        [SerializeField] private float feedbackUIStayTime;

        public Button startButton;
        [SerializeField] private float timeBetweenFadeButton;

        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private VideoClip enVideoClip;
        [SerializeField] private VideoClip trVideoClip;
        [SerializeField] private GameObject videoOnCanvas;
        [SerializeField] private GameObject skipButton;

        private int indexTablet = 0;
        private int introWatchedBefore;
        private bool isIntroPlaying = false;

        [Header("Arenas")]
        [SerializeField] private Image arena1Image;
        [SerializeField] private Sprite arena1Unlocked;
        [SerializeField] private Sprite arena1Locked;
        [SerializeField] private Image arena2Image;
        [SerializeField] private Sprite arena2Unlocked;
        [SerializeField] private Sprite arena2Locked;
        [SerializeField] private Image arena3Image;
        [SerializeField] private Sprite arena3Unlocked;
        [SerializeField] private Sprite arena3Locked;
        [SerializeField] private Image arena4Image;
        [SerializeField] private Sprite arena4Unlocked;
        [SerializeField] private Sprite arena4Locked;
        [SerializeField] private Image arena5Image;
        [SerializeField] private Sprite arena5Unlocked;
        [SerializeField] private Sprite arena5Locked;

        public List<GameObject> infoTablets = new List<GameObject>();

        [SerializeField] private List<Sprite> cardBackImages = new List<Sprite>();
        [SerializeField] private List<Sprite> cardFrontImages = new List<Sprite>();

        [SerializeField] private List<Sprite> backgroundImages = new List<Sprite>();

        [SerializeField] private float timeToMoveCurtain;
        [SerializeField] private List<GameObject> curtains = new List<GameObject>();


        [SerializeField] private float timeBetweenLockFrames;
        public float timeToWaitBeforeLock;
        [SerializeField] private List<Sprite> lockFrames = new List<Sprite>();

        [SerializeField] private List<GameObject> gods = new List<GameObject>();

        [SerializeField] private List<GameObject> levelIndicators = new List<GameObject>();
        [SerializeField] private TMP_Text playCountText;

        [SerializeField] private Image timeSliderFill;
        [SerializeField] private float flashInterval = 0.5f;
        private bool isFlashable = true;

        void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            indexTablet = GameManager.instance.GetInfoTabletIndex(LevelManager.instance.levelId);
            StartIntro();
            glowHalfCircle.GetComponent<Glow>().GlowStart();
            StartCoroutine(FadeOutAlpha());

            timeSlider.maxValue = timeRemaining;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTime();
            UpdateMoveSlider();
        }

        private void UpdateTime()
        {
            //timer continue if game is playing
            if (timeRemaining > 0 && GameManager.instance.state == GameManager.GameState.Playing)
            {
                timeRemaining -= Time.deltaTime;
            }
            //stop timer if time ran out
            else if (timeRemaining <= 0 && GameManager.instance.state == GameManager.GameState.Playing)
            {
                GameManager.instance.state = GameManager.GameState.Idle;
                //StartCoroutine(TimesUp());
                //timeRemaining = 0;
                GameManager.instance.Transition();
            }

            if (timeRemaining <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            }

            //make timer in 00 format
            timeSlider.value = timeRemaining;
        }

        #region Intro

        private void StartIntro()
        {
            StartCoroutine(IntroRoutine());
        }

        private void EndReached()
        {
            //close the gameobject and stop the video
            videoOnCanvas.SetActive(false);
            PlayerPrefs.SetInt("W92_introWatchedBefore", 1);
            skipButton.SetActive(false);
            videoPlayer.Stop();
            GameManager.instance.state = GameManager.GameState.Idle;
            GameManager.instance.totalPlayCount++;
            AudioManager.instance.Play("Background");
            isIntroPlaying = false;
            UnlockArenas();
        }

        public void SkipIntro()
        {
            CancelInvoke();
            videoPlayer.Stop();
            skipButton.SetActive(false);
            videoOnCanvas.SetActive(false);
            DOTween.KillAll();
            UnlockArenas();
            GameManager.instance.state = GameManager.GameState.Idle;
            GameManager.instance.totalPlayCount++;
            AudioManager.instance.Play("Background");
            timeToWaitBeforeLock = 0.5f;
            isIntroPlaying = false;
        }

        IEnumerator IntroRoutine()
        {
            // videoPlayer.clip = LeanLocalization.Instances[0].CurrentLanguage == "English" ? enVideoClip : trVideoClip;
            // yield return new WaitForSecondsRealtime(3);
            // isIntroPlaying = true;
            // timeToWaitBeforeLock = 1.5f;
            // videoPlayer.Play();
            Invoke("EndReached", 0f);
            // Invoke("StartEnterClouds", 2f);
            introWatchedBefore = PlayerPrefs.GetInt("W92_introWatchedBefore", 0);
            introWatchedBefore = 1;
            if (introWatchedBefore == 1)
                skipButton.SetActive(true);
            yield return null;
        }

        #endregion

        public void SetPlayCountText()
        {
            playCountText.text = GameManager.instance.totalPlayCount.ToString() + "/" + LevelManager.LevelSO.totalRounds;
        }

        public void SetPlayCountText(int value)
        {
            playCountText.text = value + "/" + LevelManager.LevelSO.totalRounds;
        }

        public void StartEnterClouds()
        {
            Debug.Log("start enter clouds");
            StartCoroutine(EnterClouds());
        }

        IEnumerator EnterClouds()
        {
            if (!isIntroPlaying)
            {
                Debug.Log("isIntroPlaying değilmiş");
                AudioManager.instance.PlayOneShot("Transition");

                if (LevelSelectionCanvas.activeSelf || !infoTabletLevel)
                    AudioManager.instance.PlayOneShot("CardShuffle");
            }

            GameObject cloudL = GameManager.instance.cloudsL;
            GameObject cloudR = GameManager.instance.cloudsR;

            Tween moveLCloudsIn = cloudL.transform.DOLocalMoveX(-1270f, 1f);
            Tween moveRCloudsIn = cloudR.transform.DOLocalMoveX(1268f, 1f);

            yield return moveRCloudsIn.WaitForCompletion();

            if (isIntroPlaying)
                yield return new WaitForSecondsRealtime(1f);

            //condition needed to prevent showing tablets twice
            if (!LevelSelectionCanvas.activeSelf)
            {
                UnlockArenas();
            }

            if (LevelSelectionCanvas.activeSelf && !isIntroPlaying)
                SwitchToGameCanvas();

            //if tablet need to be seen, return to level canvas
            else if (GameCanvas.activeSelf && infoTabletLevel && GameManager.instance.totalPlayCount < LevelManager.LevelSO.totalRounds + 1)
            {
                SwitchToLevelCanvas();
            }

            //if not, play 3 times without returning to level canvas
            else if (GameCanvas.activeSelf && !infoTabletLevel && GameManager.instance.totalPlayCount < LevelManager.LevelSO.totalRounds + 1)
            {
                LevelManager.instance.ClearCards();
                GameManager.instance.state = GameManager.GameState.Idle;

                GameManager.instance.StartLevel();
                GetComponent<HighlightCircles>().StartHighlight();
            }

            else if (GameManager.instance.totalPlayCount >= LevelManager.LevelSO.totalRounds + 1)
            {
                GameManager.instance.Finish();
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.25f);

            Tween moveLCloudsOut = cloudL.transform.DOLocalMoveX(-2870f, 1f);
            Tween moveRCloudsOut = cloudR.transform.DOLocalMoveX(3036f, 1f);

            yield return moveRCloudsOut.WaitForCompletion();
            startButton.interactable = true;
        }

        public void UpdateMoveSlider()
        {
            numOfAttemptsSlider.maxValue = numOfAttempts;
            numOfAttemptsSlider.value = Mathf.SmoothStep(numOfAttemptsSlider.value, numOfAttempts - moveCounter, 0.2f);
        }

        public void SwitchToGameCanvas()
        {
            LevelSelectionCanvas.SetActive(false);
            GameCanvas.SetActive(true);
            GameManager.instance.StartLevel();
            GetComponent<HighlightCircles>().StartHighlight();
        }

        public void SwitchToLevelCanvas()
        {
            LevelManager.instance.ClearCards();
            GameCanvas.SetActive(false);
            LevelSelectionCanvas.SetActive(true);
            glowHalfCircle.GetComponent<Glow>().GlowStart();
            GameManager.instance.state = GameManager.GameState.Idle;
        }

        public void ShowCorrectUI()
        {
            CloseCorrectUI();
            CloseWrongUI();

            StartCoroutine(CorrectUI());
        }

        public void ShowWrongUI()
        {
            CloseCorrectUI();
            CloseWrongUI();

            StartCoroutine(WrongUI());
        }

        IEnumerator TimesUp()
        {
            GameManager.instance.state = GameManager.GameState.Idle;
            AnimateCamera.instance.SwitchCameraToSecond();

            //Move clouds in the middle to block view
            GameObject cloudL = GameManager.instance.cloudsL;
            GameObject cloudR = GameManager.instance.cloudsR;

            Tween moveLCloudsIn = cloudL.transform.DOLocalMoveX(-1270f, 2f);
            Tween moveRCloudsIn = cloudR.transform.DOLocalMoveX(1268f, 2f);

            yield return moveRCloudsIn.WaitForCompletion();
            GameManager.instance.Finish();
            yield break;
        }

        void UpdateLevelIndicators()
        {
            for (int i = 0; i < LevelManager.instance.levelId - 1; i++)
            {
                levelIndicators[i].GetComponent<Image>().color = new Color(0f, 0.8842978f, 1f, 1f);
            }
        }

        IEnumerator CorrectUI()
        {
            correctUI.SetActive(true);
            AudioManager.instance.PlayOneShot("Correct");
            Tween scaleTween = correctUI.transform.DOScale(feedbackUIScale, feedbackUIScaleTime);
            yield return scaleTween.WaitForCompletion();

            yield return new WaitForSeconds(feedbackUIStayTime);

            correctUI.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), feedbackUIScaleTime).OnComplete(() => correctUI.SetActive(false));
        }

        void CloseCorrectUI()
        {
            correctUI.transform.DOKill(true);
            correctUI.SetActive(false);
            StopCoroutine(CorrectUI());
        }

        IEnumerator WrongUI()
        {
            wrongUI.SetActive(true);
            AudioManager.instance.PlayOneShot("Wrong");
            Tween scaleTween = wrongUI.transform.DOScale(feedbackUIScale, feedbackUIScaleTime);
            yield return scaleTween.WaitForCompletion();

            yield return new WaitForSeconds(feedbackUIStayTime);

            wrongUI.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), feedbackUIScaleTime).OnComplete(() => wrongUI.SetActive(false));
        }

        void CloseWrongUI()
        {
            wrongUI.transform.DOKill(true);
            wrongUI.SetActive(false);
            StopCoroutine(WrongUI());
        }

        IEnumerator FadeOutAlpha()
        {
            float timeElapsed = 0;
            Color startVal = Color.white;
            while (timeElapsed < timeBetweenFadeButton)
            {
                startButton.GetComponent<Image>().color = Color.Lerp(startVal, new Color(255f, 255f, 255f, 0f), timeElapsed / timeBetweenFadeButton);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeInAlpha());
        }

        IEnumerator FadeInAlpha()
        {
            float timeElapsed = 0;
            Color startVal = Color.clear;
            while (timeElapsed < timeBetweenFadeButton)
            {
                startButton.GetComponent<Image>().color = Color.Lerp(startVal, new Color(255f, 255f, 255f, 1f), timeElapsed / timeBetweenFadeButton);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeOutAlpha());
        }

        void Unlock(GameObject lockObj)
        {
            StartCoroutine(AnimateLock(lockObj.GetComponent<Image>()));
        }

        IEnumerator AnimateLock(Image lockToUnlock)
        {
            yield return new WaitForSeconds(timeToWaitBeforeLock);

            for (int i = 0; i < lockFrames.Count; i++)
            {
                lockToUnlock.sprite = lockFrames[i];
                yield return new WaitForSeconds(timeBetweenLockFrames);
            }
            lockToUnlock.gameObject.SetActive(false);
        }

        public void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(timeSliderFill.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(timeSliderFill.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }

        public void AssignLevelVariables()
        {
            LevelManager.instance.cardAmount = LevelManager.LevelSO.numOfCards;
            numOfAttempts = LevelManager.LevelSO.moveLimit * 2;
        }

        public void UnlockArenas()
        {
            moveCounter = 0;
            timeToWaitBeforeLock = 0.5f;

            UpdateLevelIndicators();

            switch (LevelManager.instance.levelId)
            {
                //load into arena 1
                case 1:
                case 2:
                    UnlockArena1();
                    AssignLevelVariables();
                    break;

                //load into areana1GodL
                case 3:
                case 4:
                    UnlockArena1();
                    AssignLevelVariables();
                    ShowInfoTablet(0);
                    break;

                //load into arena1GodR
                case 5:
                case 6:
                    UnlockArena1();
                    AssignLevelVariables();
                    ShowInfoTablet(1);
                    break;

                //load into arena 2
                case 7:
                case 8:
                    UnlockArena1();
                    UnlockArena2();
                    OpenCurtains(2, false);
                    AssignLevelVariables();
                    break;

                case 9:
                case 10:
                    UnlockArena1();
                    UnlockArena2();
                    AssignLevelVariables();
                    ShowInfoTablet(2);
                    break;

                case 11:
                case 12:
                    UnlockArena1();
                    UnlockArena2();
                    AssignLevelVariables();
                    ShowInfoTablet(3);
                    break;

                case 13:
                case 14:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    OpenCurtains(4, false);
                    AssignLevelVariables();
                    break;

                case 15:
                case 16:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    AssignLevelVariables();
                    ShowInfoTablet(4);
                    break;

                case 17:
                case 18:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    AssignLevelVariables();
                    ShowInfoTablet(5);
                    break;

                case 19:
                case 20:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    OpenCurtains(6, false);
                    AssignLevelVariables();
                    break;

                case 21:
                case 22:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    AssignLevelVariables();
                    ShowInfoTablet(6);
                    break;

                case 23:
                case 24:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    AssignLevelVariables();
                    ShowInfoTablet(7);
                    break;

                case 25:
                case 26:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();
                    OpenCurtains(8, false);
                    AssignLevelVariables();
                    break;

                case 27:
                case 28:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();
                    AssignLevelVariables();
                    ShowInfoTablet(8);
                    break;

                case 29:
                case 30:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();
                    AssignLevelVariables();
                    ShowInfoTablet(9);
                    break;

                case 31:
                case 32:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();

                    AssignLevelVariables();
                    ShowInfoTablet(10);
                    break;

                case 33:
                case 34:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();

                    AssignLevelVariables();
                    ShowInfoTablet(11);
                    break;

                default:
                    UnlockArena1();
                    UnlockArena2();
                    UnlockArena3();
                    UnlockArena4();
                    UnlockArena5();

                    AssignLevelVariables();
                    ShowInfoTablet(11);
                    break;

            }
        }

        #region UnlockArena Methods

        void UnlockArena1()
        {
            //change arena image
            arena1Image.sprite = arena1Unlocked;
            //hide lock image
            Unlock(arena1Image.transform.GetChild(0).gameObject);
            //change cards back and front
            LevelManager.instance.cardBackImage = cardBackImages[0];
            LevelManager.instance.cardFrontImage = cardFrontImages[0];
            //change background
            gameBackground.sprite = backgroundImages[0];
        }

        void UnlockArena2()
        {
            arena2Image.sprite = arena2Unlocked;
            Unlock(arena2Image.transform.GetChild(0).gameObject);
            LevelManager.instance.cardBackImage = cardBackImages[1];
            LevelManager.instance.cardFrontImage = cardFrontImages[1];
            gameBackground.sprite = backgroundImages[1];
        }

        void UnlockArena3()
        {
            arena3Image.sprite = arena3Unlocked;
            Unlock(arena3Image.transform.GetChild(0).gameObject);
            LevelManager.instance.cardBackImage = cardBackImages[2];
            LevelManager.instance.cardFrontImage = cardFrontImages[2];
            gameBackground.sprite = backgroundImages[2];
        }

        void UnlockArena4()
        {
            arena4Image.sprite = arena4Unlocked;
            Unlock(arena4Image.transform.GetChild(0).gameObject);
            LevelManager.instance.cardBackImage = cardBackImages[3];
            LevelManager.instance.cardFrontImage = cardFrontImages[3];
            gameBackground.sprite = backgroundImages[3];
        }

        void UnlockArena5()
        {
            arena5Image.sprite = arena5Unlocked;
            Unlock(arena5Image.transform.GetChild(0).gameObject);
            LevelManager.instance.cardBackImage = cardBackImages[4];
            LevelManager.instance.cardFrontImage = cardFrontImages[4];
            gameBackground.sprite = backgroundImages[4];
        }

        #endregion

        void ShowInfoTablet(int curtainIndex)
        {
            OpenCurtains(curtainIndex);

            if (!GameCanvas.activeSelf)
                Glow(curtainIndex);

            startButton.interactable = false;
            infoTabletLevel = true;
            startTransitionFlag = true;
            indexTablet = GameManager.instance.GetInfoTabletIndex(LevelManager.instance.levelId);
            Debug.Log("indexTablet: " + indexTablet);
            infoTablets[indexTablet].SetActive(true);
        }

        void OpenCurtains(int index, bool animateFlag = true)
        {
            for (int i = 0; i < index; i++)
            {
                if (i < 10)
                {
                    RectTransform curtainRect = curtains[i].GetComponent<RectTransform>();
                    curtainRect.sizeDelta = new Vector2(0f, curtainRect.rect.height);
                }
            }

            if (index < 10 && animateFlag)
                StartCoroutine(OpenCurtain(curtains[index]));

            //last two gods seperated
            if (index == 10 && animateFlag)
                gods[10].GetComponent<Image>().color = Color.white;

            if (index == 11 && animateFlag)
            {
                gods[10].GetComponent<Image>().color = Color.white;
                gods[11].GetComponent<Image>().color = Color.white;
            }
        }

        void Glow(int indexGod)
        {
            glow.SetActive(true);
            glow.GetComponent<Glow>().GlowStart();
            glow.transform.position = gods[indexGod].transform.position;
        }

        IEnumerator OpenCurtain(GameObject curtain)
        {
            RectTransform curtainRect = curtain.GetComponent<RectTransform>();

            Tween DoSizeDeltaTween = curtainRect.DOSizeDelta(new Vector2(0f, curtainRect.rect.height), timeToMoveCurtain);
            yield return DoSizeDeltaTween.WaitForCompletion();
        }
    }
}