using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class HighlightCircles : MonoBehaviour
    {
        [SerializeField] private Image circleL;
        [SerializeField] private Image circleR;
        [SerializeField] private float timeToColor;
        [SerializeField] private float minAlpha;
        [SerializeField] private float maxAlpha;


        public void StartHighlight()
        {
            StopCoroutine(FadeOutAlpha());
            StopCoroutine(FadeInAlpha());

            StartCoroutine(FadeOutAlpha());
        }

        IEnumerator FadeOutAlpha()
        {
            float timeElapsed = 0;
            while (timeElapsed < timeToColor)
            {
                circleL.color = Color.Lerp(new Color(1f, 0.6910995f, 0f, maxAlpha), new Color(1f, 0.6910995f, 0f, minAlpha), timeElapsed / timeToColor);
                circleR.color = Color.Lerp(new Color(1f, 0.6910995f, 0f, maxAlpha), new Color(1f, 0.6910995f, 0f, minAlpha), timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeInAlpha());
        }

        IEnumerator FadeInAlpha()
        {
            float timeElapsed = 0;
            Color startVal = new Color(1f, 0.6910995f, 0f, 0.25f);
            while (timeElapsed < timeToColor)
            {
                circleL.color = Color.Lerp(new Color(1f, 0.6910995f, 0f, minAlpha), new Color(1f, 0.6910995f, 0f, maxAlpha), timeElapsed / timeToColor);
                circleR.color = Color.Lerp(new Color(1f, 0.6910995f, 0f, minAlpha), new Color(1f, 0.6910995f, 0f, maxAlpha), timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeOutAlpha());
        }
    }
}