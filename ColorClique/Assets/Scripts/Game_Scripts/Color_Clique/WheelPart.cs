using System.Collections.Generic;
using UnityEngine;

namespace Color_Clique
{
    public class WheelPart : MonoBehaviour
    {
        [SerializeField] private List<Slot> slots = new List<Slot>();

        public void Initialize(int numberOfColors, int shapeCount)
        {
            SetSlots(numberOfColors, shapeCount);
        }

        public Slot GetRandomSlot()
        {
            return slots[Random.Range(0, slots.Count)];
        }

        public void SetSlots(int numberOfColors, int shapeCount)
        {
            int assignedShapeCounter = 0;
            int assignedColorCounter = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                if (assignedShapeCounter < shapeCount)
                {
                    assignedShapeCounter++;
                    slots[i].SetItemSprite(LevelManager.instance.GetWheel().GetRandomItem());
                }
                else
                {
                    slots[i].SetItemSpriteOff();
                }
            }

            for (int i = slots.Count - 1; i >= 0; i--)
            {
                if (assignedColorCounter < numberOfColors)
                {
                    assignedColorCounter++;
                    slots[i].SetSlotColor(LevelManager.instance.GetWheel().GetRandomColor());
                }
            }
        }
    }
}