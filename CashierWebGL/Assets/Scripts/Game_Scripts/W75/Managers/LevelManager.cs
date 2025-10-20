using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Cashier
{
	public class LevelManager : MonoBehaviour
	{
		[Header("Level Variables")]
		public int levelId;
		[SerializeField] private LevelSO levelSO;
		[SerializeField] private List<LevelSO> levels = new List<LevelSO>();

		[Header("Controllers")]
		[SerializeField] private UIManager uiManager;
		[SerializeField] private BarcodeController barcodeController;

		[Header("Lists")]
		[SerializeField] private List<Sprite> productSprites = new List<Sprite>();
		private List<Sprite> chosenProducts = new List<Sprite>();

		[Header("Product")]
		[SerializeField] private Product product;
		private Sprite chosenProductSp;
		private int totalShownProducts;

		private List<int> pressedNumbers = new List<int>();
		private bool enterPressed = false;
		private int upCounter;
		private int downCounter;
		private int correct;
		private int wrong;
		public int totalCorrect;
		public int totalWrong;
		private int maxLevelWKeys;

		void Update()
		{

#if UNITY_WEBGL

			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			{
				Check();
			}

			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				Delete();
			}

#endif
		}

		void OnEnable()
		{
			GameStateManager.OnGameStateChanged += OnStateChanged;
		}

		void OnDisable()
		{
			GameStateManager.OnGameStateChanged -= OnStateChanged;
		}

		public void Initialize()
		{
			maxLevelWKeys = levels.Count;
			Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

			AudioManager.instance.Play(SoundType.Background);
			AssignLevel();
			uiManager.ShowLevelId(levelId);
			StartGame();
		}

		void StartGame()
		{
			Reset();
			barcodeController.Reset();
			product.Reset();
			uiManager.ClearDigitalScreen();
			uiManager.ClearProductOnCashbox();

			AssignLevel();

			if (totalShownProducts < levelSO.numOfItemsDisplayedPerLevel)
				SpawnProduct();
			else
				GameStateManager.SetGameState(GameState.MemoryGame);
		}

		void AssignLevel()
		{
			levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
			levelSO = levels[levelId - 1];

			uiManager.SetLevelTime(levelSO.barcodeEntryTime);
		}

		void OnStateChanged()
		{
			switch (GameStateManager.GetGameState())
			{
				case GameState.Idle:
					enterPressed = false;
					break;

				case GameState.ProductEnter:
					Reset();
					barcodeController.Reset();
					product.Reset();
					uiManager.SetCashboxTextState(true);
					uiManager.ClearDigitalScreen();
					uiManager.ClearProductOnCashbox();
					break;

				case GameState.BarcodeShow:
					OnProductMidPos(true);
					GenerateBarcode();
					break;

				case GameState.EnterBarcode:
					OnProductMidPos(false);
					uiManager.SetProductOnCashbox(chosenProductSp);
					Number.isPressable = true;
					uiManager.SetCashboxTextState(false);
					break;

				case GameState.ProductExit:
					uiManager.ClearDigitalScreen();
					Number.isPressable = false;
					DecideLevel();
					break;

				case GameState.TimesUp:
					Number.isPressable = false;
					uiManager.GiveCashboxFeedback(false, true);
					DecideLevel();
					product.Exit().OnComplete(() => StartGame());
					break;

				case GameState.MemoryGame:
					uiManager.AddRandomSpritesToQuestion(levelSO.totalNumberOfFakeItems);
					break;

				case GameState.GameOver:
					break;
			}
		}

		void SpawnProduct()
		{
			do
			{
				chosenProductSp = productSprites[Random.Range(0, productSprites.Count)];
			} while (chosenProducts.Contains(chosenProductSp));
			chosenProducts.Add(chosenProductSp);
			product.SetSprite(chosenProductSp);

			product.Enter().OnComplete(() =>
			{
				GameStateManager.SetGameState(GameState.BarcodeShow);
			});
			totalShownProducts++;
		}

		void GenerateBarcode()
		{
			int barcodeLength = levelSO.barcodeLength;
			int maxBarcodeLength = levelSO.maxBarcodeLength;
			BarcodeDigitOrder barcodeDigitOrder = (BarcodeDigitOrder)levelSO.barcodeDigitOrder;
			BarcodeDisplayFormat barcodeDisplayFormat = (BarcodeDisplayFormat)levelSO.barcodeDisplayFormat;
			barcodeController.SetBarcode(barcodeLength, maxBarcodeLength, barcodeDigitOrder, barcodeDisplayFormat);
		}

		public void NumberPressed(int number)
		{
			pressedNumbers.Add(number);
			uiManager.AddToDigitalScreen(number);
		}

		public void Delete()
		{
			if (GameStateManager.GetGameState() != GameState.EnterBarcode)
				return;

			AudioManager.instance.PlayOneShot(SoundType.OneNumber);
			if (pressedNumbers.Count > 0)
			{
				pressedNumbers.RemoveAt(pressedNumbers.Count - 1);
				uiManager.DeleteLastDigit();
			}
		}

		public void Check()
		{
			if (GameStateManager.GetGameState() != GameState.EnterBarcode)
				return;

			if (!enterPressed)
				enterPressed = true;

			AudioManager.instance.PlayOneShot(SoundType.OneNumber);
			GameStateManager.SetGameState(GameState.Idle);

			List<int> barcode = new List<int>(barcodeController.GetBarcode());

			for (int i = 0; i < barcode.Count; i++)
			{
				if (pressedNumbers.Count != barcode.Count)
				{
					Wrong();
					return;
				}

				if (barcode[i] != pressedNumbers[i])
				{
					Wrong();
					return;
				}
			}

			Correct();
		}

		public void Correct()
		{
			correct++;
			totalCorrect++;
			AudioManager.instance.PlayOneShot(SoundType.Correct);
			uiManager.GiveCashboxFeedback(true);

			DOTween.Sequence()
				.Append(product.transform.DOScale(350f, 0.5f))
				.AppendInterval(0.3f)
				.Append(product.transform.DOScale(300f, 0.3f))
				.OnComplete(() =>
				{
					product.Exit().OnComplete(() =>
					{
						StartGame();
					});
				});
		}

		public void Wrong()
		{
			wrong++;
			totalWrong++;
			float rotation = 15f;

			AudioManager.instance.PlayOneShot(SoundType.Wrong);
			uiManager.GiveCashboxFeedback(false);

			Sequence seq = DOTween.Sequence()
				.Append(product.transform.DORotate(new Vector3(0, 0, rotation), 0.1f))
				.Append(product.transform.DORotate(new Vector3(0, 0, -rotation), 0.1f))
				.SetLoops(2);

			seq.OnComplete(() =>
			{
				product.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
				product.Exit().OnComplete(() => StartGame());
			});
		}

		private void DecideLevel()
		{
			upCounter = PlayerPrefs.GetInt("Cashier_UpCounter", 0);
			downCounter = PlayerPrefs.GetInt("Cashier_DownCounter", 0);

			if (wrong < levelSO.numOfQuestionsToLevelDown)
			{
				upCounter++;

				if (upCounter >= levelSO.numOfQuestionsToLevelUp * 2)
				{
					upCounter = 0;
					downCounter = 0;
					correct = 0;
					wrong = 0;

					levelId++;
					levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
					Debug.Log($"Level changed to : {levelId}");

					uiManager.ShowLevelId(levelId);
				}
			}
			else
			{
				downCounter++;

				if (downCounter >= levelSO.numOfQuestionsToLevelDown)
				{
					downCounter = 0;
					upCounter = 0;
					correct = 0;
					wrong = 0;

					levelId--;
					levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
					Debug.Log($"Level changed to : {levelId}");

					uiManager.ShowLevelId(levelId);
				}
			}

			// Debug.Log($"upCounter : {upCounter}, downCounter : {downCounter}");
			// Debug.Log($"correct : {correct}, wrong : {wrong}");
			// Debug.Log($"numOfQuestionsToLevelUp : {levelSO.numOfQuestionsToLevelUp}, numOfQuestionsToLevelDown : {levelSO.numOfQuestionsToLevelDown}");

			PlayerPrefs.SetInt("Cashier_UpCounter", upCounter);
			PlayerPrefs.SetInt("Cashier_DownCounter", downCounter);
		}

		private void OnProductMidPos(bool state)
		{
			barcodeController.SetScanImage(state);

			if (state)
				AudioManager.instance.PlayOneShot(SoundType.Barcode);
		}

		private void Reset()
		{
			pressedNumbers.Clear();
			enterPressed = false;
			uiManager.ClearLevelTimer();
		}

		public int CalculateScore()
		{
			int score = (totalCorrect * levelSO.pointsPerCorrectAnswer) - (totalWrong * levelSO.penaltyPoints);
			score = Mathf.Clamp(score, 0, levelSO.maxInGame);
			score = Mathf.CeilToInt((float)score / levelSO.maxInGame * 1000);
			score = Mathf.Clamp(score, 0, 1000);
			return score;
		}
	}
}