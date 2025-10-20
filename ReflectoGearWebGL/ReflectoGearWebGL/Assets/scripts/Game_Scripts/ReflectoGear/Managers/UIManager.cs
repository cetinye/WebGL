using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace W91_ReflectoGear
{
    public class UIManager : MonoBehaviour
    {
        public float timeToColor;
        public bool updateProgressbarFlag = true;
        public TextMeshProUGUI nextText;
        public int counterIndicator = 0;

        [SerializeField] private int countdownTime;
        [SerializeField] private float timeRemaining;
        [SerializeField] private float timeToMove;
        [SerializeField] private GameObject upLid;
        [SerializeField] private GameObject downLid;
        [SerializeField] private GameObject upLidFinalPos;
        [SerializeField] private GameObject downLidFinalPos;
        [SerializeField] private float timeToOpenLidAtStart;
        [SerializeField] private Transform upLidOpenPos;
        [SerializeField] private Transform downLidOpenPos;
        [SerializeField] private GameObject indicatorCircle;
        [SerializeField] private TextMeshProUGUI levelNo;
        [SerializeField] private TextMeshProUGUI time;
        [SerializeField] private Image bottomGearUp;
        [SerializeField] private Image bottomGearDown;
        // [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject videoOnCanvas;
        [SerializeField] private GameObject skipButton;
        [SerializeField] private ParticleSystem smokeParticle;
        [SerializeField] private float smokeEveryXTime;
        [SerializeField] private GameObject bottombars;

        private float distanceUpLid;
        private float distanceDownLid;
        private float newUpPos;
        private float newDownPos;
        private bool UplidRoutineRunning = false;
        private bool DownlidRoutineRunning = false;
        private bool isRedFinished = true;
        private bool isGreenFinished = true;
        private int introWatchedBefore;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelManager levelManager;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("PlaySmoke", 3f, smokeEveryXTime);

            newUpPos = upLid.transform.localPosition.y;
            newDownPos = downLid.transform.localPosition.y;
        }

        private void Update()
        {
            UpdateTime();
        }

        public void UpdateLevelNo()
        {
            timeRemaining = 60f;

            levelNo.text = (levelManager.levelId + 1).ToString();
        }

        public void UpdateBottomGearImage()
        {
            bottomGearDown.sprite = levelManager.level.gearOnBottomDown;
            bottomGearUp.sprite = levelManager.level.gearOnBottomUp;
        }

        public void UpdateProgressBar()
        {
            //increment counter everytime correct move is played and progressbar update
            counterIndicator++;

            //calculate the distance between starting pos and end pos only once for each level
            if (updateProgressbarFlag)
            {
                updateProgressbarFlag = false;
                distanceUpLid = (upLidFinalPos.transform.localPosition.y - upLidOpenPos.localPosition.y) / levelManager.AnswerList.Count;
                distanceDownLid = (downLidOpenPos.localPosition.y - downLidFinalPos.transform.localPosition.y) / levelManager.AnswerList.Count;
            }

            //add the distance needs to be travelled 
            newUpPos = upLidOpenPos.localPosition.y + distanceUpLid * counterIndicator;
            newDownPos = downLidOpenPos.localPosition.y - distanceDownLid * counterIndicator;

            if (UplidRoutineRunning == true)
            {
                StopCoroutine(UplidLerp());
                UplidRoutineRunning = false;
            }
            StartCoroutine(UplidLerp());

            if (DownlidRoutineRunning == true)
            {
                StopCoroutine(DownlidLerp());
                DownlidRoutineRunning = false;
            }
            StartCoroutine(DownlidLerp());
        }

        private void UpdateTime()
        {
            //timer continue if game is playing
            if (timeRemaining > 0 && gameManager.state == GameManager.GameState.Playing)
            {
                timeRemaining -= Time.deltaTime;
            }
            //stop timer if time ran out
            else if (timeRemaining <= 0 && gameManager.state == GameManager.GameState.Playing)
            {
                gameManager.state = GameManager.GameState.Failed;
                timeRemaining = 0;
                StartCoroutine(levelManager.AnimateUnloadLevel());
            }

            //make timer in 0:00 format
            time.text = string.Format("{0:00}", timeRemaining);
        }

        #region Intro

        public void StartIntro()
        {
            Debug.Log("StartIntro");
            SkipIntro();
            // StartCoroutine(IntroRoutine());
        }

        private void EndReached()
        {
            //close the gameobject and stop the video
            videoOnCanvas.SetActive(false);
            PlayerPrefs.SetInt("W91_introWatchedBefore", 1);
            skipButton.SetActive(false);
            // videoPlayer.Stop();
            gameManager.state = GameManager.GameState.Idle;
            AudioManager.instance.Play("BackgroundMusic");
            levelManager.StartGameRoutine();
            bottombars.GetComponent<BottomBars>().EnableParticle();
        }

        public void SkipIntro()
        {
            Debug.Log("SkipIntro");
            CancelInvoke();
            Debug.Log("SkipIntro2");
            // videoPlayer.Stop();
            skipButton.SetActive(false);
            Debug.Log("SkipIntro3");
            videoOnCanvas.SetActive(false);
            Debug.Log("SkipIntro4");
            gameManager.state = GameManager.GameState.Idle;
            Debug.Log("SkipIntro5");
            AudioManager.instance.Play("BackgroundMusic");
            Debug.Log("SkipIntro6");
            levelManager.StartGameRoutine();
            Debug.Log("SkipIntro7");
            bottombars.GetComponent<BottomBars>().EnableParticle();
            Debug.Log("SkipIntro8");
        }

        IEnumerator IntroRoutine()
        {
            yield return new WaitForSecondsRealtime(3);
            // videoPlayer.Play();
            // Invoke("EndReached", (float)videoPlayer.clip.length);
            introWatchedBefore = PlayerPrefs.GetInt("W91_introWatchedBefore", 0);
            if (introWatchedBefore == 1)
                skipButton.SetActive(true);
        }

        #endregion

        IEnumerator UplidLerp()
        {
            UplidRoutineRunning = true;
            float timeElapsed = 0;
            Vector3 startVal = upLid.transform.localPosition;
            while (timeElapsed < timeToMove)
            {
                upLid.transform.localPosition = Vector3.Lerp(startVal, new Vector3(upLid.transform.localPosition.x, newUpPos, upLid.transform.localPosition.z), timeElapsed / timeToMove);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            upLid.transform.localPosition = new Vector3(upLid.transform.localPosition.x, newUpPos, upLid.transform.localPosition.z);
            UplidRoutineRunning = false;
        }

        IEnumerator DownlidLerp()
        {
            DownlidRoutineRunning = true;
            float timeElapsed = 0;
            Vector3 startVal = downLid.transform.localPosition;
            while (timeElapsed < timeToMove)
            {
                downLid.transform.localPosition = Vector3.Lerp(startVal, new Vector3(downLid.transform.localPosition.x, newDownPos, downLid.transform.localPosition.z), timeElapsed / timeToMove);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            downLid.transform.localPosition = new Vector3(downLid.transform.localPosition.x, newDownPos, downLid.transform.localPosition.z);
            DownlidRoutineRunning = false;
        }

        public IEnumerator OpenLid()
        {
            newUpPos = upLid.transform.localPosition.y;
            newDownPos = downLid.transform.localPosition.y;

            float timeElapsed = 0;
            Vector3 startValUp = upLid.transform.localPosition;
            Vector3 startValDown = downLid.transform.localPosition;
            while (timeElapsed < timeToMove)
            {
                upLid.transform.localPosition = Vector3.Lerp(startValUp, upLidOpenPos.localPosition, timeElapsed / timeToOpenLidAtStart);
                downLid.transform.localPosition = Vector3.Lerp(startValDown, downLidOpenPos.localPosition, timeElapsed / timeToOpenLidAtStart);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            upLid.transform.localPosition = upLidOpenPos.localPosition;
            downLid.transform.localPosition = downLidOpenPos.localPosition;
        }

        public void LightRed()
        {
            if (isRedFinished)
                StartCoroutine(TurnRed());
            else
            {
                StopCoroutine(TurnRed());
                StopCoroutine(TurnWhite());
                StartCoroutine(TurnRed());
            }
        }

        public void LightGreen()
        {
            Gear.isTappable = true;
            if (isGreenFinished)
                StartCoroutine(TurnGreen());
            else
            {
                StopCoroutine(TurnGreen());
                StopCoroutine(TurnWhite());
                StartCoroutine(TurnGreen());
            }
        }

        IEnumerator TurnRed()
        {
            isRedFinished = false;
            float timeElapsed = 0;
            Color orgColor = indicatorCircle.GetComponent<Image>().color;
            while (timeElapsed < timeToMove)
            {
                indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.red, timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            indicatorCircle.GetComponent<Image>().color = Color.red;
            StartCoroutine(TurnWhite());
        }

        IEnumerator TurnGreen()
        {
            isGreenFinished = false;
            float timeElapsed = 0;
            Color orgColor = indicatorCircle.GetComponent<Image>().color;
            while (timeElapsed < timeToMove)
            {
                indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.green, timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            indicatorCircle.GetComponent<Image>().color = Color.green;
            StartCoroutine(TurnWhite());
        }

        IEnumerator TurnWhite()
        {
            float timeElapsed = 0;
            Color orgColor = indicatorCircle.GetComponent<Image>().color;
            while (timeElapsed < timeToMove)
            {
                indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.white, timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            indicatorCircle.GetComponent<Image>().color = Color.white;
            isRedFinished = true;
            isGreenFinished = true;
        }

        public void PlaySmoke()
        {
            smokeParticle.Play();
            AudioManager.instance.PlayOneShot("Steam");
        }
    }
}