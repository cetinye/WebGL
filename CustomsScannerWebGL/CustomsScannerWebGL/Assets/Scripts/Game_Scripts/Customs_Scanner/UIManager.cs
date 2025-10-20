using DG.Tweening;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Customs_Scanner
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text playCountText;

        [Space(20)]
        [Header("Common Screen Variables")]
        public RectTransform middlePos;
        public RectTransform outPos;
        public float tabletMoveTime;
        [SerializeField] private Image blackScreen;
        [SerializeField] private float blackScreenFadeOutTime;

        [Space(20)]
        [Header("Item List Screen Variables")]
        public GameObject listPanel;
        public Transform forbiddenItemsParent;
        [Range(0, 7)] public int forbiddenItemsCount;
        public float timeToShowItemsList;
        [SerializeField] private float fadeTime;
        [SerializeField] private Image listBackground;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text desc;
        [SerializeField] private TMP_Text successRateText;
        [SerializeField] private TMP_Text totalProhibitedText;
        [SerializeField] private TMP_Text detectedProhibitedText;
        [SerializeField] private TMP_Text totalItemsInspected;
        [SerializeField] private TMP_Text cargoFillPercentageWas;
        [SerializeField] private TMP_Text moreChallengingTasks;

        [SerializeField] private TMP_Text listText;
        public TMP_Text itemTimerText;
        [SerializeField] private TMP_Text fillPercentageText;
        public Button startButton;
        [SerializeField] private Image startButtonImage;
        [SerializeField] private TMP_Text startButtonText;
        public Button endButton;
        [SerializeField] private Image endButtonImage;
        [SerializeField] private TMP_Text endButtonText;
        public float itemTimer;
        public List<Sprite> forbiddenItemsList = new List<Sprite>();
        public Image itemImage;
        public TMP_Text itemText;
        public bool isItemTimerActive = false;

        [Space(20)]
        [Header("Stat Screen Variables")]
        public GameObject statPanel;
        [SerializeField] private float timeToShowStatScreen;
        [SerializeField] private Image statScreenBackground;
        [SerializeField] private Sprite statScreenWinBackground, statScreenFailBackground;
        [SerializeField] private TMP_Text rateText;
        [SerializeField] private TMP_Text totalProhibitedItemsText;
        [SerializeField] private TMP_Text detectedProhibitedItemsText;
        [SerializeField] private TMP_Text totalItemsInspectedText;
        [SerializeField] private TMP_Text cargoFillPercentageText;

        [Space(20)]
        [Header("XRay Light Variables")]
        [SerializeField] private SpriteRenderer xraySmallLight;
        [SerializeField] private SpriteRenderer xrayBigLight;
        [SerializeField] private Sprite xraySmallCorrect, xraySmallWrong;
        [SerializeField] private Sprite xrayBigCorrect, xrayBigWrong;
        [SerializeField] private float xrayLightDuration;

        public void SetLevelText(int level)
        {
            levelText.text = $"{LeanLocalization.GetTranslationText("Level")} {level}";
        }

        public void SetPlayCountText(int playCount, int maxPlayCount)
        {
            playCountText.text = playCount + "/" + maxPlayCount;
        }

        public void SetXrayLight(bool state)
        {
            if (state)
            {
                levelManager.correctCount++;
                levelManager.correct++;
                xraySmallLight.sprite = xraySmallCorrect;
                xrayBigLight.sprite = xrayBigCorrect;
            }
            else
            {
                levelManager.wrongCount++;
                levelManager.wrong++;
                xraySmallLight.sprite = xraySmallWrong;
                xrayBigLight.sprite = xrayBigWrong;
            }
            StopCoroutine(TurnSmallLightDefault());
            xraySmallLight.gameObject.SetActive(true);
            xrayBigLight.gameObject.SetActive(true);
            StartCoroutine(TurnSmallLightDefault());
            StartCoroutine(TurnBigLightDefault());
        }

        public void ShowTabletStats()
        {
            statPanel.SetActive(true);
            StartCoroutine(ShowTabletStatsRoutine());
        }

        IEnumerator SetItemsAnim(float toFadeVal, float fadeTime, float moveBelowAmount, float timeBetweenItemAnim)
        {
            Tween itemSlide;
            RectTransform item;

            for (int i = 0; i < forbiddenItemsCount; i++)
            {
                item = forbiddenItemsParent.GetChild(i).GetComponent<RectTransform>();
                itemImage = item.GetChild(0).GetComponent<Image>();
                itemText = item.GetChild(1).GetComponent<TMP_Text>();

                itemImage.DOFade(toFadeVal, fadeTime);
                itemText.DOFade(toFadeVal, fadeTime);
                float destination = item.anchoredPosition.y + moveBelowAmount;
                itemSlide = item.DOAnchorPosY(destination, timeBetweenItemAnim);

                yield return itemSlide.WaitForCompletion();
            }
        }

        public void ShowItemList()
        {
            StartCoroutine(ShowItemListRoutine());
        }

        IEnumerator ShowItemListRoutine()
        {
            listPanel.SetActive(true);

            fillPercentageText.text = $"{LeanLocalization.GetTranslationText("FillPercentage")}" + levelManager.level.productFillPercent;
            itemTimerText.text = itemTimer.ToString();

            blackScreen.gameObject.SetActive(true);
            Tween blackScr = blackScreen.DOFade(0f, blackScreenFadeOutTime);
            yield return blackScr.WaitForCompletion();
            blackScreen.gameObject.SetActive(false);

            listPanel.GetComponent<RectTransform>().DOAnchorPos(outPos.anchoredPosition, 0f);

            listBackground.DOFade(1f, 0f);

            AudioManager.instance.PlayOneShot("TabletMove");
            Tween move = listPanel.GetComponent<RectTransform>().DOAnchorPos3D(middlePos.anchoredPosition, tabletMoveTime);
            yield return move.WaitForCompletion();

            listText.DOFade(1f, fadeTime);
            itemTimerText.DOFade(1f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            StartCoroutine(SetItemsAnim(1f, fadeTime, 200f, (fadeTime / 2) + 0.10f));
            yield return new WaitForSeconds(((fadeTime / 2) + 0.10f) * levelManager.level.forbiddenItemAmount);
            fillPercentageText.DOFade(1f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            startButtonImage.DOFade(1f, fadeTime);
            startButtonText.DOFade(1f, fadeTime);
            startButton.interactable = true;
            isItemTimerActive = true;
            yield return new WaitForSeconds(timeToShowItemsList);

            AudioManager.instance.PlayOneShot("TabletMove");
            listPanel.GetComponent<RectTransform>().DOAnchorPos(outPos.anchoredPosition, tabletMoveTime);
            listBackground.DOFade(0f, fadeTime);
            listText.DOFade(0f, fadeTime);
            itemTimerText.DOFade(0f, fadeTime);
            startButtonImage.DOFade(0f, fadeTime);
            startButtonText.DOFade(0f, fadeTime);
            startButton.interactable = false;
            Tween fade = fillPercentageText.DOFade(0f, fadeTime);
            StartCoroutine(SetItemsAnim(0f, fadeTime, 0f, 0f));
            yield return fade.WaitForCompletion();
            listPanel.SetActive(false);
        }

        IEnumerator ShowTabletStatsRoutine()
        {
            Tween fade;

            int tempRate = 0;
            float rate = 0f;
            rate = (((float)levelManager.correctCount - (float)levelManager.wrongCount) / (float)levelManager.totalPassedForbiddenProductAmount) * 100f;
            rate = Mathf.FloorToInt(rate);

            if (rate >= levelManager.successRateRequired)
            {
                statScreenBackground.sprite = statScreenWinBackground;
                title.text = LeanLocalization.GetTranslationText("Congratulations");
                desc.text = LeanLocalization.GetTranslationText("WinDesc");
                moreChallengingTasks.text = LeanLocalization.GetTranslationText("WinMoreChallengeAwaits");
                rateText.color = new Color(0.145098f, 0.6901961f, 0.4784314f, 1f);
            }
            else
            {
                statScreenBackground.sprite = statScreenFailBackground;
                title.text = LeanLocalization.GetTranslationText("TryAgain");
                desc.text = LeanLocalization.GetTranslationText("LoseDesc");
                moreChallengingTasks.text = LeanLocalization.GetTranslationText("LoseYouNeedToCatch");
                rateText.color = new Color(0.7333333f, 0.2f, 0.2f, 1f);
            }

            statPanel.GetComponent<RectTransform>().DOAnchorPos(outPos.anchoredPosition, 0f);
            statScreenBackground.DOFade(0f, 0f);
            rateText.DOFade(0f, 0f);

            statScreenBackground.DOFade(1f, 0f);

            title.DOFade(1f, 0f);
            desc.DOFade(1f, 0f);
            successRateText.DOFade(1f, 0f);
            totalProhibitedText.DOFade(1f, 0f);
            detectedProhibitedText.DOFade(1f, 0f);
            totalItemsInspected.DOFade(1f, 0f);
            cargoFillPercentageWas.DOFade(1f, 0f);
            moreChallengingTasks.DOFade(1f, 0f);

            AudioManager.instance.PlayOneShot("TabletMove");
            Tween move = statPanel.GetComponent<RectTransform>().DOAnchorPos(middlePos.anchoredPosition, tabletMoveTime);
            yield return move.WaitForCompletion();

            fade = rateText.DOFade(1f, fadeTime);
            yield return fade.WaitForCompletion();

            rateText.text = "%" + tempRate;
            while (tempRate < rate)
            {
                tempRate++;
                rateText.text = "%" + tempRate;
                yield return new WaitForSeconds(0.01f);
            }

            levelManager.DecideOnLevel(rate);

            yield return new WaitForSeconds(fadeTime / 2);

            totalProhibitedItemsText.text = levelManager.totalPassedForbiddenProductAmount.ToString();
            fade = totalProhibitedItemsText.DOFade(1f, fadeTime / 2);
            yield return fade.WaitForCompletion();

            detectedProhibitedItemsText.text = levelManager.correctCount.ToString();
            fade = detectedProhibitedItemsText.DOFade(1f, fadeTime / 2);
            yield return fade.WaitForCompletion();

            totalItemsInspectedText.text = levelManager.totalPassedProductAmount.ToString();
            fade = totalItemsInspectedText.DOFade(1f, fadeTime / 2);
            yield return fade.WaitForCompletion();

            cargoFillPercentageText.text = levelManager.productFillPercent.ToString();
            fade = cargoFillPercentageText.DOFade(1f, fadeTime / 2);
            yield return fade.WaitForCompletion();

            endButtonImage.DOFade(1f, fadeTime / 2);
            endButtonText.DOFade(1f, fadeTime / 2);
            endButton.interactable = true;

            yield return new WaitForSeconds(timeToShowStatScreen);

            AudioManager.instance.PlayOneShot("TabletMove");
            move = statPanel.GetComponent<RectTransform>().DOAnchorPos(outPos.anchoredPosition, tabletMoveTime);
            yield return move.WaitForCompletion();
            statPanel.SetActive(false);
            levelManager.EndGame();
        }

        IEnumerator TurnSmallLightDefault()
        {
            yield return new WaitForSeconds(xrayLightDuration);
            xraySmallLight.gameObject.SetActive(false);
        }

        IEnumerator TurnBigLightDefault()
        {
            yield return new WaitForSeconds(xrayLightDuration);
            xrayBigLight.gameObject.SetActive(false);
        }
    }
}