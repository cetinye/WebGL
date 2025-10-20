using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cashier
{
	public class BarcodeController : MonoBehaviour
	{
		[SerializeField] private List<int> barcode;
		[SerializeField] private BarcodeDigit barcodeDigitPrefab;
		[SerializeField] private HorizontalLayoutGroup layoutGroup;
		[SerializeField] private GameObject barcodePanel;
		[SerializeField] private Image barcodeImg;
		[SerializeField] private TMP_Text barcodeText;
		[SerializeField] private Image grayPanel;
		[SerializeField] private Image scanImg;
		[SerializeField] private float barcodeTimeToFade;
		[SerializeField] private float grayTimeToFade;
		private List<BarcodeDigit> barcodeDigits = new List<BarcodeDigit>();
		private int barcodeLength;
		private int maxBarcodeLength;
		private BarcodeDigitOrder barcodeDigitOrder;
		private BarcodeDisplayFormat barcodeDisplayFormat;

		public void SetBarcode(int barcodeLength, int maxBarcodeLength, BarcodeDigitOrder barcodeDigitOrder, BarcodeDisplayFormat barcodeDisplayFormat)
		{
			this.barcodeLength = barcodeLength;
			this.maxBarcodeLength = maxBarcodeLength;
			this.barcodeDigitOrder = barcodeDigitOrder;
			this.barcodeDisplayFormat = barcodeDisplayFormat;
			this.barcode = new List<int>(CreateBarcode());
			Generate();
		}

		private List<int> CreateBarcode()
		{
			List<int> barcode = new List<int>();
			for (int i = 0; i < barcodeLength; i++)
			{
				barcode.Add(Random.Range(1, 10));
			}
			return barcode;
		}

		public void Generate()
		{
			for (int i = 0; i < maxBarcodeLength; i++)
			{
				BarcodeDigit digit = Instantiate(barcodeDigitPrefab, layoutGroup.transform);
				barcodeDigits.Add(digit);
			}

			for (int i = 0; i < barcode.Count; i++)
			{
				if (i <= barcode.Count - 1)
					barcodeDigits[i].SetDigit(barcode[i]);
			}

			ShowBarcode();
		}

		public List<int> GetBarcode()
		{
			return barcode;
		}

		public void ShowBarcode()
		{
			switch (barcodeDisplayFormat)
			{
				case BarcodeDisplayFormat.Single:
					StartCoroutine(ShowSingleBarcodeRoutine());
					break;

				case BarcodeDisplayFormat.Double:
					StartCoroutine(ShowDoubleBarcodeRoutine());
					break;

				case BarcodeDisplayFormat.Triple:
					StartCoroutine(ShowTripleBarcodeRoutine());
					break;
			}
		}

		private Tween SetBarcodeState(bool state)
		{
			barcodeText.DOFade(state ? 1 : 0, barcodeTimeToFade);
			return barcodeImg.DOFade(state ? 1 : 0, barcodeTimeToFade);
		}

		private Sequence GrayPanel(bool state)
		{
			Sequence s = DOTween.Sequence();
			s.Append(grayPanel.DOFade(state ? 0.5f : 0, grayTimeToFade));
			return s;
		}

		private void FadeDigits(bool state)
		{
			for (int i = 0; i < barcodeDigits.Count; i++)
			{
				barcodeDigits[i].SetState(state, barcodeTimeToFade);
			}
		}

		public void Reset()
		{
			for (int i = 0; i < barcodeDigits.Count; i++)
			{
				Destroy(barcodeDigits[i].gameObject);
			}

			barcodeDigits.Clear();
			barcode.Clear();
			barcodePanel.SetActive(false);
		}

		public void SetScanImage(bool state)
		{
			scanImg.gameObject.SetActive(state);
		}

		#region Routines

		IEnumerator ShowSingleBarcodeRoutine()
		{
			Sequence s = GrayPanel(true);
			yield return s.WaitForCompletion();

			barcodePanel.SetActive(true);
			Tween b = SetBarcodeState(true);
			FadeDigits(true);
			yield return b.WaitForCompletion();

			int indexToShow;
			List<int> shownIndexes = new List<int>();

			// repeat for all barcode digits
			for (int i = 0; i < barcode.Count; i++)
			{
				// sequential
				indexToShow = i;

				// if not sequential get random index
				if (barcodeDigitOrder == BarcodeDigitOrder.Random)
				{
					do
					{
						indexToShow = Random.Range(0, barcode.Count);
					} while (shownIndexes.Contains(indexToShow));
					shownIndexes.Add(indexToShow);
				}
				else
				{
					shownIndexes.Add(indexToShow);
				}

				// show digit
				AudioManager.instance.PlayOneShot(SoundType.OneNumber);
				Tween t = barcodeDigits[indexToShow].Show();
				yield return t.WaitForCompletion();

				yield return new WaitForSeconds(0.2f);
			}

			b = SetBarcodeState(false);
			FadeDigits(false);
			yield return b.WaitForCompletion();
			barcodePanel.SetActive(false);

			s = GrayPanel(false);
			yield return s.WaitForCompletion();

			GameStateManager.SetGameState(GameState.EnterBarcode);
		}

		IEnumerator ShowDoubleBarcodeRoutine()
		{
			Sequence s = GrayPanel(true);
			yield return s.WaitForCompletion();

			barcodePanel.SetActive(true);
			Tween b = SetBarcodeState(true);
			FadeDigits(true);
			yield return b.WaitForCompletion();

			int indexToShow_1, indexToShow_2;
			List<int> shownIndexes = new List<int>();

			// repeat for all barcode digits
			for (int i = 0; i < barcode.Count; i += 2)
			{
				// sequential
				indexToShow_1 = i;
				indexToShow_2 = i + 1;

				// if sequentialRandom
				if (barcodeDigitOrder == BarcodeDigitOrder.SequentialRandom)
				{
					do
					{
						indexToShow_1 = Random.Range(0, barcode.Count);
					} while (shownIndexes.Contains(indexToShow_1) || indexToShow_1 % 2 == 1);
					shownIndexes.Add(indexToShow_1);

					indexToShow_2 = indexToShow_1 + 1;
					if (indexToShow_2 <= barcode.Count - 1)
					{
						indexToShow_2 = indexToShow_1 + 1;
						shownIndexes.Add(indexToShow_2);
					}
				}

				// if not sequential get random index
				else if (barcodeDigitOrder == BarcodeDigitOrder.Random)
				{
					do
					{
						indexToShow_1 = Random.Range(0, barcode.Count);
					} while (shownIndexes.Contains(indexToShow_1));
					shownIndexes.Add(indexToShow_1);

					indexToShow_2 = indexToShow_1 + 1;
					if (indexToShow_2 <= barcode.Count - 1)
					{
						do
						{
							indexToShow_2 = Random.Range(0, barcode.Count);
						} while (shownIndexes.Contains(indexToShow_2));
						shownIndexes.Add(indexToShow_2);
					}
				}

				else if (barcodeDigitOrder == BarcodeDigitOrder.Sequential)
				{
					shownIndexes.Add(indexToShow_1);
					shownIndexes.Add(indexToShow_2);
				}

				// play related sound
				if (indexToShow_1 <= barcode.Count - 1 && indexToShow_2 <= barcode.Count - 1)
					AudioManager.instance.PlayOneShot(SoundType.TwoNumber);

				else if (indexToShow_1 <= barcode.Count - 1)
					AudioManager.instance.PlayOneShot(SoundType.OneNumber);

				// show digit
				Tween t_1 = barcodeDigits[indexToShow_1].Show();

				if (indexToShow_2 <= barcode.Count - 1)
				{
					// time to wait before showing other digit
					yield return new WaitForSeconds(0.28f);
					Tween t_2 = barcodeDigits[indexToShow_2].Show();
					yield return t_2.WaitForCompletion();
				}
				else
				{
					yield return t_1.WaitForCompletion();
				}

				yield return new WaitForSeconds(0.2f);
			}

			b = SetBarcodeState(false);
			FadeDigits(false);
			yield return b.WaitForCompletion();
			barcodePanel.SetActive(false);

			s = GrayPanel(false);
			yield return s.WaitForCompletion();

			GameStateManager.SetGameState(GameState.EnterBarcode);
		}

		IEnumerator ShowTripleBarcodeRoutine()
		{
			Sequence s = GrayPanel(true);
			yield return s.WaitForCompletion();

			barcodePanel.SetActive(true);
			Tween b = SetBarcodeState(true);
			FadeDigits(true);
			yield return b.WaitForCompletion();

			int indexToShow_1, indexToShow_2, indexToShow_3;
			List<int> shownIndexes = new List<int>();

			// repeat for all barcode digits
			for (int i = 0; i < barcode.Count; i += 3)
			{
				// sequential
				indexToShow_1 = i;
				indexToShow_2 = i + 1;
				indexToShow_3 = i + 2;

				// if sequentialRandom
				if (barcodeDigitOrder == BarcodeDigitOrder.SequentialRandom)
				{
					do
					{
						indexToShow_1 = Random.Range(0, barcode.Count);
					} while (shownIndexes.Contains(indexToShow_1) || indexToShow_1 % 3 != 0);
					shownIndexes.Add(indexToShow_1);

					if (!shownIndexes.Contains(indexToShow_1 + 1) && indexToShow_1 + 1 <= barcode.Count - 1)
					{
						indexToShow_2 = indexToShow_1 + 1;
						shownIndexes.Add(indexToShow_2);
					}
					else
						indexToShow_2 = 99;

					if (!shownIndexes.Contains(indexToShow_2 + 1) && indexToShow_2 + 1 <= barcode.Count - 1)
					{
						indexToShow_3 = indexToShow_2 + 1;
						shownIndexes.Add(indexToShow_3);
					}
					else
						indexToShow_3 = 99;
				}

				// if not sequential get random index
				else if (barcodeDigitOrder == BarcodeDigitOrder.Random)
				{
					do
					{
						indexToShow_1 = Random.Range(0, barcode.Count);
					} while (shownIndexes.Contains(indexToShow_1));
					shownIndexes.Add(indexToShow_1);

					if (indexToShow_2 <= barcode.Count - 1)
					{
						do
						{
							indexToShow_2 = Random.Range(0, barcode.Count);
						} while (shownIndexes.Contains(indexToShow_2));
						shownIndexes.Add(indexToShow_2);
					}

					if (indexToShow_3 <= barcode.Count - 1)
					{
						do
						{
							indexToShow_3 = Random.Range(0, barcode.Count);
						} while (shownIndexes.Contains(indexToShow_3));
						shownIndexes.Add(indexToShow_3);
					}
				}

				else if (barcodeDigitOrder == BarcodeDigitOrder.Sequential)
				{
					shownIndexes.Add(indexToShow_1);
					shownIndexes.Add(indexToShow_2);
					shownIndexes.Add(indexToShow_3);
				}

				// play related sound
				if (indexToShow_1 <= barcode.Count - 1 && indexToShow_2 <= barcode.Count - 1 && indexToShow_3 <= barcode.Count - 1)
					AudioManager.instance.PlayOneShot(SoundType.ThreeNumber);

				else if (indexToShow_1 <= barcode.Count - 1 && indexToShow_2 <= barcode.Count - 1)
					AudioManager.instance.PlayOneShot(SoundType.TwoNumber);

				else if (indexToShow_1 <= barcode.Count - 1)
					AudioManager.instance.PlayOneShot(SoundType.OneNumber);

				// show digit
				Tween t_1 = barcodeDigits[indexToShow_1].Show();

				if (indexToShow_2 <= barcode.Count - 1)
				{
					// time to wait before showing other digit
					yield return new WaitForSeconds(0.28f);
					Tween t_2 = barcodeDigits[indexToShow_2].Show();

					if (indexToShow_3 <= barcode.Count - 1)
					{
						yield return new WaitForSeconds(0.28f);
						Tween t_3 = barcodeDigits[indexToShow_3].Show();
						yield return t_3.WaitForCompletion();
					}
					else
					{
						yield return t_2.WaitForCompletion();
					}
				}
				else
				{
					yield return t_1.WaitForCompletion();
				}

				yield return new WaitForSeconds(0.2f);
			}

			b = SetBarcodeState(false);
			FadeDigits(false);
			yield return b.WaitForCompletion();
			barcodePanel.SetActive(false);

			s = GrayPanel(false);
			yield return s.WaitForCompletion();

			GameStateManager.SetGameState(GameState.EnterBarcode);
		}

		#endregion
	}
}
