using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using TMPro;
using Lean.Localization;

namespace Customs_Scanner
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        public int levelId;
        public int score;
        public LevelSO level;

        [Header("Lists")]
        [SerializeField] private List<LevelSO> levelList = new List<LevelSO>();
        [SerializeField] private List<Vehicle> vehicleList = new List<Vehicle>();
        [SerializeField] private List<ProductSO> productList = new List<ProductSO>();

        [Space(20)]
        [Header("Variables")]
        public int vehicleAmount;
        private int spawnedVehicleCount = 0;
        public int successRateRequired;
        public int correctCount;
        public int wrongCount;
        public int correct;
        public int wrong;
        public int scoreMultiplier;
        public int totalPassedForbiddenProductAmount;
        public int totalPassedProductAmount;
        [SerializeField] private Transform levelObjects;
        public bool isTimerOn = false;
        public float Timer => levelTimer;
        [SerializeField] private float levelTimer;
        private float remainingTime;
        private bool isEndGameRunning = false;
        private List<Product> totalActiveProds = new List<Product>();
        private int downCounter;
        private int upCounter;

        [Space(20)]
        [Header("Vehicle Variables")]
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private float vehicleMoveTime;
        public int productFillPercent;
        [SerializeField] private int totalBoxAmount;
        [SerializeField] private int totalProductAmount;
        [SerializeField] private int forbiddenBoxAmount;
        private float vehicleStartXPos;
        private float vehicleEndXPos;
        private int maxLevelWKeys;

        public void StartGame()
        {
            // remainingTime = WManagers.WRCM.GetGamePlaytime();
            remainingTime = 90;

            maxLevelWKeys = levelList.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            AudioManager.instance.Play("Background");
            AssignLevelVariables();
            SetItemList();
        }

        private void Update()
        {
            StartScreenTimer();
        }

        private void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            level = levelList[levelId - 1];

            vehicleMoveTime = level.vehicleMoveTime;
            productFillPercent = level.productFillPercent;
            uiManager.forbiddenItemsCount = level.forbiddenItemAmount;
            forbiddenBoxAmount = level.forbiddenBoxAmount;
            vehicleAmount = level.vehicleAmount;
            uiManager.timeToShowItemsList = level.timeToShowItemList;
            uiManager.itemTimer = uiManager.timeToShowItemsList;

            levelTimer = remainingTime;

            uiManager.listPanel.SetActive(false);
            uiManager.statPanel.SetActive(false);

            uiManager.SetLevelText(levelId);
            uiManager.SetPlayCountText(spawnedVehicleCount, vehicleAmount);
        }

        private void StartScreenTimer()
        {
            if (!uiManager.isItemTimerActive)
                return;

            uiManager.itemTimer -= Time.deltaTime;
            uiManager.itemTimerText.text = uiManager.itemTimer.ToString("F0");

            if (uiManager.itemTimer < 0f)
            {
                uiManager.isItemTimerActive = false;

                //start if not started yet
                if (!isTimerOn)
                {
                    isTimerOn = true;
                    GenerateVehicle();
                }
            }
        }

        private void GenerateVehicle()
        {
            //dont spawn new vehicle if limit reached
            if (spawnedVehicleCount >= vehicleAmount)
            {
                uiManager.ShowTabletStats();
                return;
            }

            spawnedVehicleCount++;
            uiManager.SetPlayCountText(spawnedVehicleCount, vehicleAmount);
            GetRandomVehicle();
            SetAvailableProducts();
            SetForbiddenProducts();
            MoveVehicle();
        }

        private void GetRandomVehicle()
        {
            int randomVehicleIndex = Random.Range(0, vehicleList.Count);
            vehicle = Instantiate(vehicleList[randomVehicleIndex], levelObjects);

            vehicle.timeToCompleteMove = vehicleMoveTime;
            vehicleStartXPos = vehicle.vehicleStartXPos;
            vehicleEndXPos = vehicle.vehicleEndXPos;
        }

        private void SetAvailableProducts()
        {
            int randProductIndex, randBoxIndex, randChildIndex;

            totalBoxAmount = vehicle.boxList.Count;
            totalProductAmount = vehicle.totalProductAmount;
            totalProductAmount = Mathf.CeilToInt(totalProductAmount * productFillPercent / 100f);

            int numOfFilledProduct = 0;
            while (numOfFilledProduct < totalProductAmount)
            {
                //only assign not forbidden items & on empty spaces
                do
                {
                    //assign in a random box
                    randBoxIndex = Random.Range(0, totalBoxAmount);

                    //assign in a random spot inside box
                    randChildIndex = Random.Range(0, vehicle.boxList[randBoxIndex].transform.childCount);

                    //get random product
                    randProductIndex = Random.Range(0, productList.Count);

                } while (uiManager.forbiddenItemsList.Contains(productList[randProductIndex].productSprite)
                        || vehicle.boxList[randBoxIndex].productList[randChildIndex].spriteRenderer.sprite != null);

                vehicle.boxList[randBoxIndex].productList[randChildIndex].spriteRenderer.sprite = productList[randProductIndex].productSprite;
                vehicle.boxList[randBoxIndex].productList[randChildIndex].gameObject.SetActive(true);
                numOfFilledProduct++;
            }
        }

        private void SetForbiddenProducts()
        {
            int randBoxIndex, randChildIndex;

            totalPassedForbiddenProductAmount += forbiddenBoxAmount;

            for (int i = 0; i < forbiddenBoxAmount; i++)
            {
                //prevent replacing the product which is already set as forbidden
                do
                {
                    //assign in a random box
                    randBoxIndex = Random.Range(0, totalBoxAmount);

                    //assign in a random spot inside box
                    randChildIndex = Random.Range(0, vehicle.boxList[randBoxIndex].transform.childCount);

                } while (vehicle.boxList[randBoxIndex].productList[randChildIndex].isForbiddenProduct == true
                        || vehicle.boxList[randBoxIndex].productList[randChildIndex].spriteRenderer == null);

                //only assign forbidden items
                int randProductIndex = Random.Range(0, uiManager.forbiddenItemsList.Count);
                vehicle.boxList[randBoxIndex].productList[randChildIndex].spriteRenderer.sprite = uiManager.forbiddenItemsList[randProductIndex];
                vehicle.boxList[randBoxIndex].productList[randChildIndex].isForbiddenProduct = true;
                vehicle.boxList[randBoxIndex].productList[randChildIndex].gameObject.SetActive(true);
            }

            if (level.isSecretItemEnabled)
                SetSecretProduct();
        }

        private void SetSecretProduct()
        {
            int chance = Random.Range(1, 101);

            if (chance <= 30)
            {
                int randProductIndex = Random.Range(0, uiManager.forbiddenItemsList.Count);
                vehicle.secretProduct.spriteRenderer.sprite = uiManager.forbiddenItemsList[randProductIndex];
                vehicle.secretProduct.isForbiddenProduct = true;
                vehicle.secretProduct.gameObject.SetActive(true);
            }
        }

        private int GetActiveProductsCount()
        {
            totalActiveProds.Clear();
            totalActiveProds = FindObjectsOfType<Product>().ToList();
            return totalActiveProds.Count;
        }

        private void SetItemList()
        {
            int randIndex;

            //prevent index out of bounds
            if (uiManager.forbiddenItemsCount > uiManager.forbiddenItemsParent.childCount)
                uiManager.forbiddenItemsCount = uiManager.forbiddenItemsParent.childCount;

            for (int i = 0; i < uiManager.forbiddenItemsCount; i++)
            {
                uiManager.itemImage = uiManager.forbiddenItemsParent.GetChild(i).GetChild(0).GetComponent<Image>();
                uiManager.itemText = uiManager.forbiddenItemsParent.GetChild(i).GetChild(1).GetComponent<TMP_Text>();

                //avoid duplicated items
                do
                {
                    randIndex = Random.Range(0, productList.Count);
                    uiManager.itemImage.sprite = productList[randIndex].productSprite;

                    //add space between uppercase characters, skip CPU
                    string itemString = productList[randIndex].productName.ToString();
                    itemString = LeanLocalization.GetTranslationText(itemString);
                    if (!itemString.Equals("CPU"))
                        itemString = string.Concat(itemString.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

                    uiManager.itemText.text = itemString;

                } while (uiManager.forbiddenItemsList.Contains(productList[randIndex].productSprite));

                uiManager.itemText.transform.parent.gameObject.SetActive(true);
                uiManager.forbiddenItemsList.Add(uiManager.itemImage.sprite);
            }

            uiManager.ShowItemList();
        }

        private void MoveVehicle()
        {
            vehicle.transform.position = new Vector3(vehicleStartXPos, vehicle.transform.position.y, vehicle.transform.position.z);

            vehicle.transform.DOMoveX(vehicle.vehicleStartSlowXPos, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                vehicle.transform.DOMoveX(vehicle.vehicleStartFastXPos, vehicleMoveTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    vehicle.transform.DOMoveX(vehicleEndXPos, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        if (vehicle != null)
                        {
                            totalPassedProductAmount += GetActiveProductsCount();
                            vehicle.StopWheels();
                            Destroy(vehicle.gameObject);
                        }
                        GenerateVehicle();
                    });
                });
            });
        }

        public int CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * level.penaltyPoints));
            levelScore = Mathf.Clamp(levelScore, 0, level.maxInGame);

            // scoreText.text = $"Score: {levelScore}";

            correct = 0;
            wrong = 0;

            return levelScore;
        }

        public void DecideOnLevel(float rate)
        {
            score = CalculateLevelScore();
            downCounter = PlayerPrefs.GetInt("CustomsScanner_DownCounter", 0);
            upCounter = PlayerPrefs.GetInt("CustomsScanner_UpCounter", 0);

            if (rate >= successRateRequired)
            {
                upCounter++;

                if (score >= level.minScore)
                {
                    if (upCounter >= 2)
                    {
                        LevelUp();
                    }
                }
            }
            else
            {
                downCounter++;

                if (downCounter >= level.levelDownCriteria)
                {
                    LevelDown();
                }
            }

            PlayerPrefs.SetInt("CustomsScanner_DownCounter", downCounter);
            PlayerPrefs.SetInt("CustomsScanner_UpCounter", upCounter);
        }

        private void LevelUp()
        {
            levelId++;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            uiManager.SetLevelText(levelId);

            downCounter = 0;
            upCounter = 0;
        }

        private void LevelDown()
        {
            levelId--;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            uiManager.SetLevelText(levelId);

            downCounter = 0;
            upCounter = 0;
        }

        public void EndGame()
        {
            isEndGameRunning = true;

            GameManager.instance.Finish();
        }

        public void StartScreenButton()
        {
            uiManager.startButton.interactable = false;
            uiManager.isItemTimerActive = false;
            AudioManager.instance.PlayOneShot("Button");
            StartCoroutine(StartScreenButtonRoutine());
        }

        public void EndScreenButton()
        {
            if (isEndGameRunning)
                return;

            AudioManager.instance.PlayOneShot("Button");
            uiManager.endButton.interactable = false;
            isEndGameRunning = true;
            StartCoroutine(EndScreenButtonRoutine());
        }

        IEnumerator EndScreenButtonRoutine()
        {
            Tween move = uiManager.statPanel.GetComponent<RectTransform>().DOAnchorPos(uiManager.outPos.anchoredPosition, uiManager.tabletMoveTime);
            yield return move.WaitForCompletion();
            uiManager.statPanel.SetActive(false);
            EndGame();
        }

        IEnumerator StartScreenButtonRoutine()
        {
            Tween move = uiManager.listPanel.GetComponent<RectTransform>().DOAnchorPos(uiManager.outPos.anchoredPosition, uiManager.tabletMoveTime).SetEase(Ease.OutQuad);
            yield return move.WaitForCompletion();
            uiManager.listPanel.SetActive(false);

            //start if not started yet
            if (!isTimerOn)
            {
                isTimerOn = true;
                GenerateVehicle();
            }

        }
    }
}