using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Customs_Scanner
{
    public class Vehicle : MonoBehaviour
    {
        public List<Box> boxList = new List<Box>();
        public List<Sprite> spriteList = new List<Sprite>();
        public List<Wheel> wheels = new List<Wheel>();
        public Product secretProduct;
        public int totalProductAmount;
        public float timeToCompleteMove;
        public float vehicleStartXPos;
        public float vehicleEndXPos;
        public float vehicleStartSlowXPos;
        public float vehicleStartFastXPos;
        public SpriteRenderer spriteRenderer;

        private void Start()
        {
            PaintToRandomColor();

            foreach (Wheel wheel in wheels)
            {
                wheel.turnTime = timeToCompleteMove / 4;
                wheel.TurnWheel();
            }
        }

        private void PaintToRandomColor()
        {
            spriteRenderer.sprite = spriteList[UnityEngine.Random.Range(0, spriteList.Count)];
        }

        public void StopWheels()
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.transform.DOKill();
            }
        }
    }
}