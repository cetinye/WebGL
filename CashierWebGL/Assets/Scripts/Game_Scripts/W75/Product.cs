using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Cashier
{
	public class Product : MonoBehaviour
	{
		[SerializeField] private Image prodImg;
		[SerializeField] private RectTransform enter;
		[SerializeField] private RectTransform middle;
		[SerializeField] private RectTransform exit;
		[SerializeField] private float moveTime;
		private RectTransform rect;

		void Awake()
		{
			rect = GetComponent<RectTransform>();
		}

		public void SetSprite(Sprite sprite)
		{
			prodImg.sprite = sprite;
		}

		public void Reset()
		{
			rect.anchoredPosition = enter.anchoredPosition;
		}

		public Tween Enter()
		{
			GameStateManager.SetGameState(GameState.ProductEnter);
			return rect.DOAnchorPos(middle.anchoredPosition, moveTime);
		}

		public Tween Exit()
		{
			GameStateManager.SetGameState(GameState.ProductExit);
			return rect.DOAnchorPos(exit.anchoredPosition, moveTime);
		}
	}
}
