using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class Glow : MonoBehaviour
    {
        [SerializeField] private float timeToColor;

        // Start is called before the first frame update
        public void GlowStart()
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
                GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0.25f), timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeInAlpha());
        }

        IEnumerator FadeInAlpha()
        {
            float timeElapsed = 0;
            while (timeElapsed < timeToColor)
            {
                GetComponent<Image>().color = Color.Lerp(new Color(1f, 1f, 1f, 0.25f), new Color(1f, 1f, 1f, 1f), timeElapsed / timeToColor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(FadeOutAlpha());
        }
    }
}