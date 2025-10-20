using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    /// <summary>
    /// Superclass for managing all the different objects in a level. This class communicates with the GameManager singleton.
    /// Handles the initialization processes when the level is loaded.
    /// </summary>
    public class LevelBehaviour : MonoBehaviour
    {
        public int levelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public static LevelSO LevelSO;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text scoreText;

        private BoatBehaviour _boat;
        private FishController _fishController;
        private GarbageController _garbageController;
        private BlurController _blurController;
        private SoundController _soundController;
        private FeedbackPanel _feedbackPanel;

        private int _currentCaughtGarbageCount;
        private int _currentSunkGarbageCount;

        private int garbageCaught;
        private int fishCaught;

        private Coroutine _comboRoutine;
        private Coroutine _fishPanelRoutine;
        private int maxLevelWKeys;

        #region Unity Methods

        private void OnDestroy()
        {
            Unsubscribe();
        }

        #endregion

        public void AssignLevel()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];

            levelText.text = "Level: " + levelId;
        }

        #region Public Methods
        public void Load()
        {
            _boat = GetComponentInChildren<BoatBehaviour>();
            if (!_boat)
                throw new LevelException($"Level \"{name}\" is missing a BoatBehaviour object");

            _fishController = GetComponentInChildren<FishController>();
            if (!_fishController)
                throw new LevelException($"Level \"{name}\" is missing a FishController object");

            _garbageController = GetComponentInChildren<GarbageController>();
            if (!_garbageController)
                throw new LevelException($"Level \"{name}\" is missing a GarbageController object");

            _blurController = GetComponentInChildren<BlurController>();
            if (!_blurController)
                throw new LevelException($"Level \"{name}\" is missing a BlurController object");

            _soundController = GetComponentInChildren<SoundController>();
            if (!_soundController)
                throw new LevelException($"Level \"{name}\" is missing a SoundController object");

            _feedbackPanel = GetComponentInChildren<FeedbackPanel>();
            if (!_feedbackPanel)
                throw new LevelException($"Level \"{name}\" is missing a ComboTextController object");

            _boat.Initialize();
            _fishController.Initialize();
            _garbageController.Initialize();
            _blurController.ResetAmount();
        }

        public void StartLevel()
        {
            Subscribe();

            _fishController.gameObject.SetActive(true);
            _garbageController.Spawning = true;
            _fishController.Spawning = true;
            _boat.Active = true;
            _feedbackPanel.Disable();

            _soundController.PlaySound(SoundType.Ambient);
        }

        public void FinishLevel()
        {
            Unsubscribe();

            _garbageController.OnFinish();
            _fishController.OnFinish();
            _boat.OnFinish();
            _soundController.StopAllSounds();
        }
        #endregion

        #region Helpers
        private void UpdateBlur()
        {
            //Debug.Log("Garbage Count:" + _garbageController.GarbageCount);
            var amount = (float)_garbageController.GarbageCount / LevelSO.maxGarbageCount;
            _blurController.SetAmount(amount);
            GameManager.Instance.SetBlurAmount(amount);
        }
        #endregion

        #region Event Functions
        private void Subscribe()
        {
            _boat.FishHookLaunched += OnFishHookLaunched;
            _boat.ItemCaught += OnItemCaught;
            _boat.ItemCollected += OnItemCollected;
            _garbageController.GarbageSpawned += OnGarbageSpawned;
            _garbageController.GarbageSunk += OnGarbageSunk;
        }

        private void Unsubscribe()
        {
            _boat.FishHookLaunched -= OnFishHookLaunched;
            _boat.ItemCaught -= OnItemCaught;
            _boat.ItemCollected -= OnItemCollected;
            _garbageController.GarbageSpawned -= OnGarbageSpawned;
            _garbageController.GarbageSunk -= OnGarbageSunk;
        }

        private void OnFishHookLaunched()
        {
            _soundController.PlaySound(SoundType.HookStart);
            _soundController.PlaySound(SoundType.HookDuring);
        }

        private void OnItemCaught(List<CatchableItem> items)
        {
            if (items.Count < 1)
                return;

            var isFish = items.First() is Fish;
            _soundController.PlaySound(isFish ? SoundType.FishCaught : SoundType.GarbageCaught);
            if (!isFish)
                return;

            if (_comboRoutine != null)
                StopCoroutine(_comboRoutine);
            _fishPanelRoutine = StartCoroutine(FishPanelRoutine());
        }

        private void OnItemCollected(List<CatchableItem> items)
        {
            var garbageCount = 0;
            foreach (var item in items)
            {
                if (item is Garbage garbage)
                {
                    garbageCount++;
                    garbageCaught++;
                    _currentCaughtGarbageCount++;
                    GameManager.Instance.DecreaseGarbageCount();

                    if (garbage.StaticGarbage)
                    {
                        GameManager.Instance.AddTime(LevelSO.extraTime);
                    }

                    if (_currentCaughtGarbageCount >= 2)
                    {
                        _currentCaughtGarbageCount = 0;
                    }

                    garbage.transform.position = -100f * Vector3.one;

                    if (!garbage.StaticGarbage)
                        _garbageController.RemoveGarbage(garbage);

                    int upCounter = PlayerPrefs.GetInt("GarbageHunt_UpCounter", 0);
                    if (++upCounter >= LevelSO.levelUpCriteria * 2)
                    {
                        levelId++;
                        upCounter = 0;
                        AssignLevel();
                    }
                    PlayerPrefs.SetInt("GarbageHunt_UpCounter", upCounter);

                }
                else if (item is Fish fish)
                {
                    fishCaught++;

                    int downCounter = PlayerPrefs.GetInt("GarbageHunt_DownCounter", 0);
                    if (++downCounter >= LevelSO.levelDownCriteria * 2)
                    {
                        levelId--;
                        downCounter = 0;
                        AssignLevel();
                    }
                    PlayerPrefs.SetInt("GarbageHunt_DownCounter", downCounter);

                    fish.ResetFish();
                }
            }

            _soundController.StopSound(SoundType.HookDuring);
            UpdateBlur();
            _soundController.PlayComboSound(garbageCount);

            scoreText.text = "Score: " + CalculateScore();

            if (garbageCount < 2)
                return;

            if (_comboRoutine != null)
            {
                StopCoroutine(_comboRoutine);
                _boat.DisableComboBonus();
                _feedbackPanel.Disable();
            }

            if (_fishPanelRoutine != null)
                StopCoroutine(_fishPanelRoutine);
            _comboRoutine = StartCoroutine(ComboRoutine(garbageCount));

        }

        public int CalculateScore()
        {
            int score = (garbageCaught * 100) - (fishCaught * LevelSO.penaltyPoints);
            score = Mathf.Max(score, 0);
            score = Mathf.CeilToInt((float)score / LevelSO.pointsPerCorrect * 1000);
            return Mathf.Clamp(score, 0, 1000);
        }

        private void OnGarbageSpawned()
        {
            GameManager.Instance.IncrementTotalGarbageCount();
            UpdateBlur();
        }

        private void OnGarbageSunk()
        {
            _currentSunkGarbageCount++;
            if (_currentSunkGarbageCount >= 2)
            {
                _currentSunkGarbageCount = 0;
            }

            if (GameManager.Instance.BlurAmount >= 1f)
                GameManager.Instance.FinishGame(false);
        }
        #endregion

        #region Coroutines

        private IEnumerator ComboRoutine(int combo)
        {
            _boat.EnableComboBonus(combo);
            _feedbackPanel.DisplayCombo(combo);
            yield return new WaitForSeconds(_feedbackPanel.Duration);
            _feedbackPanel.Disable();
            yield return new WaitForSeconds(
                GameManager.Instance.ComboBonusDuration - _feedbackPanel.Duration);
            _boat.DisableComboBonus();

        }

        private IEnumerator FishPanelRoutine()
        {
            _feedbackPanel.DisplayFishPanel();
            yield return new WaitForSeconds(_feedbackPanel.Duration);
            _feedbackPanel.Disable();
        }
        #endregion
    }

    public class LevelException : Exception
    {
        public LevelException(string s) : base(s) { }
    }
}