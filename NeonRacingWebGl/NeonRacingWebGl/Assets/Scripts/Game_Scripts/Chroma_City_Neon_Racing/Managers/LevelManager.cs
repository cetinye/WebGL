using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;
        [SerializeField] private UIManager uiManager;

        [Header("Level Variables")] public int levelId;

        [SerializeField] private LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new();

        public int score;

        [Header("Components")]
        [SerializeField]
        private Player player;

        [SerializeField] private RoadGenerator roadGenerator;
        [SerializeField] private GameObject checkpointGenerator;

        [Header("Instantiate Prefabs")]
        [SerializeField]
        private TrafficLight trafficLightPref;

        [SerializeField] private FinishLine finishPref;
        [SerializeField] private SpecialPowerUp shieldPowerUpPref;
        [SerializeField] private SpecialPowerUp speedPowerUpPref;
        [SerializeField] private SpecialPowerUp timePowerUpPref;

        [Header("Flash Interval")]
        [SerializeField]
        private bool isFlashable = true;

        private readonly List<SpecialPowerUp> spawnedSpecialPowerUps = new();
        private readonly List<Vector3> usedPositions = new();

        private float ballSpeedChangeAmount;
        private int durationOfPowerups;
        private FinishLine finish;
        private bool isLevelTimerOn;
        private float levelTimer;
        private float maxScore;
        private float maxSpeed;
        private float minSpeed;
        private int pathLength;
        private int shieldPowerup;
        private float speedPenatlyAmount;
        private int speedPowerup;
        private int timeLimit;
        private int timePowerup;
        public int maxLevelWKeys;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            LevelTimer();
            uiManager.UpdateSpeedMeter(player.GetFollowSpeed());
            uiManager.UpdateDebugTexts(GameStateManager.GetGameState().ToString(), levelId, roadGenerator.pointAmount,
                player.GetFollowSpeed());

#if UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RightPressed();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LeftPressed();
            }
#endif
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            DOTween.KillAll();
            CancelInvoke();
            isLevelTimerOn = false;

            // DeleteScene();
        }

        public void StartGame()
        {
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
            GameStateManager.SetGameState(GameState.Idle);

            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            CameraFollow.instance.Reset();

            AssignLevelVariables();

            roadGenerator.SpawnLevel();

            ColorCheckpoints();
            SpawnSpecialPowerUps();
            SpawnTrafficLight();
        }

        private void DeleteScene()
        {
            // player.Reset();
            roadGenerator.Reset();
            usedPositions.Clear();
            spawnedSpecialPowerUps.Clear();
        }

        private void DecideLevel()
        {
            if (player.WrongPowerUpsPickedUp >= 1)
            {
                var downCounter = PlayerPrefs.GetInt("CCNR_DownCounter", 0);
                if (++downCounter >= 1)
                {
                    downCounter = 0;
                    --levelId;
                    levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
                }
                PlayerPrefs.SetInt("CCNR_DownCounter", downCounter);
            }
            else
            {
                var upCounter = PlayerPrefs.GetInt("CCNR_UpCounter", 0);
                if (++upCounter >= 2)
                {
                    upCounter = 0;
                    ++levelId;
                    levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
                }
                PlayerPrefs.SetInt("CCNR_UpCounter", upCounter);
            }
        }

        private void LevelTimer()
        {
            if (!isLevelTimerOn) return;

            levelTimer -= Time.deltaTime;
            uiManager.UpdateTimer(levelTimer);

            if (levelTimer <= 0)
            {
                isLevelTimerOn = false;
                levelTimer = 0;
                uiManager.UpdateTimer(levelTimer);
                GameStateManager.SetGameState(GameState.Failed);
            }

            if (levelTimer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }
        }

        private void OnGameStateChanged()
        {
            switch (GameStateManager.GetGameState())
            {
                case GameState.Idle:
                    break;

                case GameState.Racing:
                    isLevelTimerOn = true;
                    player.SetTargetSpeed(minSpeed);
                    break;

                case GameState.Failed:
                    isLevelTimerOn = false;
                    break;

                case GameState.Success:
                    isLevelTimerOn = false;
                    player.SetTargetSpeed(minSpeed);
                    CameraFollow.instance.DetachFromPlayer();
                    break;
            }
        }

        private void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            Debug.Log("Current Level ID: " + levelId);
            levelSO = levels[levelId - 1];

            minSpeed = levelSO.minSpeedRange;
            maxSpeed = levelSO.maxSpeedRange;
            ballSpeedChangeAmount = levelSO.ballSpeedChangeAmount;
            speedPenatlyAmount = levelSO.speedPenatlyAmount;
            pathLength = levelSO.pathLength;

            shieldPowerup = levelSO.shieldPowerup;
            speedPowerup = levelSO.speedPowerup;
            timePowerup = levelSO.timePowerup;
            durationOfPowerups = levelSO.durationOfPowerups;

            timeLimit = levelSO.timeLimit;
            maxScore = levelSO.maxScore;

            player.SetSpeedChangeAmount(ballSpeedChangeAmount);
            player.SetSpeedPenaltyAmount(speedPenatlyAmount);
            player.SetMinMaxSpeed(minSpeed, maxSpeed);
            roadGenerator.SetPathLength(pathLength);
            levelTimer = timeLimit;
        }

        public void LevelFinished()
        {
            DecideLevel();
            GameManager.instance.Finish();
        }

        public int CalculateScore()
        {
            score = Mathf.CeilToInt(roadGenerator.passedPointCount * 20 + player.CorrectPowerUpsPickedUp * 50 +
                                    levelTimer * 2 -
                                    (Mathf.Max(levelSO.pathLength - roadGenerator.passedPointCount, 0) * 10 +
                                     player.WrongPowerUpsPickedUp * 25));

            score = Mathf.Clamp(score, 0, Mathf.CeilToInt(maxScore));

            var convertToWitminaScore = score / maxScore * 1000f;
            var witminaScore = Mathf.Clamp(Mathf.CeilToInt(convertToWitminaScore), 0, 1000);

            Debug.LogWarning("------------------------------------------");
            Debug.LogWarning("------------------------------------------");
            Debug.LogWarning("------------------------------------------");
            Debug.LogWarning("------------------------------------------");
            Debug.LogWarning("passedPointCount: " + roadGenerator.passedPointCount);
            Debug.LogWarning("remainingPointCount: " + (levelSO.pathLength - roadGenerator.passedPointCount));
            Debug.LogWarning("timeLeft: " + levelTimer);
            Debug.LogWarning("CorrectPowerUpsPickedUp: " + player.CorrectPowerUpsPickedUp);
            Debug.LogWarning("WrongPowerUpsPickedUp: " + player.WrongPowerUpsPickedUp);
            Debug.LogWarning("Score: " + score);
            Debug.LogWarning("convertToWitminaScore: " + convertToWitminaScore);
            Debug.LogWarning("witminaScore: " + witminaScore);
            Debug.LogWarning("roadGenerator.passedPointCount * 20: " + roadGenerator.passedPointCount * 20);
            Debug.LogWarning("player.CorrectPowerUpsPickedUp * 50: " + player.CorrectPowerUpsPickedUp * 50);
            Debug.LogWarning("levelTimer * 3: " + levelTimer * 2);
            Debug.LogWarning("------------------------------------------");
            Debug.LogWarning("levelSO.pathLength - roadGenerator.passedPointCount: " +
                             Mathf.Max(levelSO.pathLength - roadGenerator.passedPointCount, 0));
            Debug.LogWarning("levelSO.pathLength - roadGenerator.passedPointCount * 10: " +
                             Mathf.Max(levelSO.pathLength - roadGenerator.passedPointCount, 0) * 10);
            Debug.LogWarning("player.WrongPowerUpsPickedUp * 25: " + player.WrongPowerUpsPickedUp * 25);
            Debug.LogWarning("------------------------------------------");

            uiManager.UpdateScoreTexts(witminaScore, witminaScore);

            return witminaScore;
        }

        public int GetCorrectCount()
        {
            return player.CorrectPowerUpsPickedUp;
        }

        public int GetWrongCount()
        {
            return player.WrongPowerUpsPickedUp;
        }

        private void ColorCheckpoints()
        {
            var checkpoints = new List<Checkpoint>();

            for (var i = 0; i < checkpointGenerator.transform.childCount; i++)
                checkpoints.Add(checkpointGenerator.transform.GetChild(i).GetComponent<Checkpoint>());

            foreach (var checkpoint in checkpoints)
            {
                checkpoint.Initialize();
                checkpoint.SetRandomColor();
            }

            player.SetColor(checkpoints[0].GetRandomColor());
        }

        public void DisableCheckpointOverlaps()
        {
            var checkpoints = new List<Checkpoint>();

            for (var i = 0; i < checkpointGenerator.transform.childCount; i++)
                checkpoints.Add(checkpointGenerator.transform.GetChild(i).GetComponent<Checkpoint>());

            foreach (var checkpoint in checkpoints) checkpoint.Initialize();
        }

        private void SpawnTrafficLight()
        {
            Instantiate(trafficLightPref, roadGenerator.transform);
        }

        public void StartTrafficLight()
        {
            AudioManager.instance.Play(SoundType.Background);
            AudioManager.instance.PlayOneShot(SoundType.MotorStart);

            TrafficLight.instance.StartCountdown();
        }

        private void SpawnSpecialPowerUps()
        {
            var rotation = Quaternion.Euler(-90f, 0f, 0f);

            var shieldPowerupCount = Random.Range(1, 4);
            var speedPowerupCount = Random.Range(1, 4);
            var timePowerupCount = Random.Range(1, 4);

            if (shieldPowerup == 1)
                for (var i = 0; i < shieldPowerupCount; i++)
                {
                    var shield = Instantiate(shieldPowerUpPref, GetRandomPointPos(), rotation, roadGenerator.transform);
                    shield.SetDuration(levelSO.durationOfPowerups);
                    spawnedSpecialPowerUps.Add(shield);
                }

            if (speedPowerup == 1)
                for (var i = 0; i < speedPowerupCount; i++)
                {
                    var speed = Instantiate(speedPowerUpPref, GetRandomPointPos(), rotation, roadGenerator.transform);
                    speed.SetDuration(levelSO.durationOfPowerups);
                    speed.SetAddSpeedAmount(1f);
                    spawnedSpecialPowerUps.Add(speed);
                }

            if (timePowerup == 1)
                for (var i = 0; i < timePowerupCount; i++)
                {
                    var time = Instantiate(timePowerUpPref, GetRandomPointPos(), rotation, roadGenerator.transform);
                    time.SetDuration(levelSO.durationOfPowerups);
                    time.SetTimeToAdd(5f);
                    spawnedSpecialPowerUps.Add(time);
                }

            Invoke(nameof(DisableOverlappingPowerups), 2.5f);
        }

        private void DisableOverlappingPowerups()
        {
            var childList = roadGenerator.splineComputerPowerUps.transform.GetComponentsInChildren<PowerUps>(false);

            for (var i = 0; i < childList.Length; i++) childList[i].DisableOverlaps();
        }

        private Vector3 GetRandomPointPos()
        {
            Vector3 pos;
            do
            {
                pos = roadGenerator.GetRandomPointPos();
            } while (usedPositions.Contains(pos));

            usedPositions.Add(pos);

            pos = new Vector3(pos.x + 0.02f, 0.102f, pos.z + Random.Range(0.2f, 0.55f));
            return pos;
        }

        public void SpawnFinish()
        {
            var childs = checkpointGenerator.transform.GetComponentsInChildren<Transform>(false);

            Transform lastActiveCheckpoint = null;

            foreach (var child in childs)
                if (child.gameObject.activeSelf)
                    lastActiveCheckpoint = child;

            if (lastActiveCheckpoint != null)
            {
                finish = Instantiate(finishPref);
                finish.transform.position = lastActiveCheckpoint.position;
                lastActiveCheckpoint.gameObject.SetActive(false);
                finish.StartLightChange();
            }
        }

        public Vector3 GetFinishPosition()
        {
            return finish.transform.position;
        }

        public void AddTime(float timeToAdd)
        {
            levelTimer += timeToAdd;
        }

        #region Input Controls

        public void RightPressed()
        {
            Debug.LogWarning("Right");
            player.SwitchLane(true);
        }

        public void LeftPressed()
        {
            Debug.LogWarning("Left");
            player.SwitchLane(false);
        }

        #endregion
    }
}