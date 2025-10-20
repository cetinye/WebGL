using UnityEngine.UI;

namespace Unity_CS
{
    public static class Unity_CSScrollRect
    {
        public static float _GetValue(this ScrollRect scrollRect)
        {
            return scrollRect.horizontal
                ? scrollRect.horizontalNormalizedPosition
                : scrollRect.verticalNormalizedPosition;
        }

        public static void _SetValue(this ScrollRect scrollRect, float value)
        {
            if (scrollRect.horizontal)
            {
                scrollRect.horizontalNormalizedPosition = value;
            }
            else
            {
                scrollRect.verticalNormalizedPosition = value;
            }
        }
    }
}