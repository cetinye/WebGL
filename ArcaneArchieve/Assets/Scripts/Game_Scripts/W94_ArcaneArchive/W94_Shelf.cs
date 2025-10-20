using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class W94_Shelf : MonoBehaviour
{
    [SerializeField] private float fadeTime;
    [SerializeField] private ParticleSystem outlineParticle;
    [SerializeField] private ParticleSystem glowParticle;

    private GameObject bookL;
    private GameObject bookM;
    private GameObject bookR;

    [SerializeField] private GameObject slotL;
    [SerializeField] private GameObject slotM;
    [SerializeField] private GameObject slotR;

    private Image bookLImage;
    private Image bookMImage;
    private Image bookRImage;

    private int bookCount = 0;

    void GetBooksInSlots()
    {
        bookCount = 0;

        //slot left
        if (slotL.transform.childCount > 0)
        {
            bookL = transform.GetChild(0).GetChild(0).gameObject;
            bookCount++;
        }

        //slot middle
        if (slotM.transform.childCount > 0)
        {
            bookM = transform.GetChild(1).GetChild(0).gameObject;
            bookCount++;
        }

        //slot right
        if (slotR.transform.childCount > 0)
        {
            bookR = transform.GetChild(2).GetChild(0).gameObject;
            bookCount++;
        }
    }

    public void CheckCombination()
    {
        GetBooksInSlots();

        if (bookL != null)
        {
            bookLImage = bookL.GetComponent<Image>();
        }

        if (bookM != null)
        {
            bookMImage = bookM.GetComponent<Image>();
        }

        if (bookR != null)
        {
            bookRImage = bookR.GetComponent<Image>();
        }

        if (bookCount == 3 &&
            bookLImage.sprite == bookMImage.sprite && 
            bookRImage.sprite == bookMImage.sprite)
        {
            W94_AudioManager.instance.PlayOneShot("Particle");

            DestroyBooks();
        }
    }

    public void CheckIfShelfEmptied()
    {
        GetBooksInSlots();

        if (bookCount == 0)
            MoveBooksToFront();
    }

    void DestroyBooks()
    {
        bookL.GetComponent<CanvasGroup>().interactable = false;
        bookM.GetComponent<CanvasGroup>().interactable = false;
        bookR.GetComponent<CanvasGroup>().interactable = false;

        W94_GameManager.instance.RemoveFromBooksList(bookL);
        W94_GameManager.instance.RemoveFromBooksList(bookM);
        W94_GameManager.instance.RemoveFromBooksList(bookR);

        outlineParticle.Play();
        glowParticle.Play();

        bookLImage.transform.DOLocalMoveX(bookLImage.transform.position.x + 32f, fadeTime / 2);
        bookLImage.DOFade(0f, fadeTime).OnComplete(() => {
            Destroy(bookL);
            bookL = null;
        });

        bookMImage.DOFade(0f, fadeTime).OnComplete(() => {
            Destroy(bookM);
            bookM = null;
        });

        bookRImage.transform.DOLocalMoveX(bookLImage.transform.position.x - 32f, fadeTime / 2);
        bookRImage.DOFade(0f, fadeTime).OnComplete(() => {
            Destroy(bookR);
            bookR = null;

            bookCount = 0;
            MoveBooksToFront();
            W94_GameManager.instance.CheckEndLevel();
        });
    }

    public void MoveBooksToFront()
    {
        GameObject backSlotL = slotL.GetComponent<W94_Slot>().backSlot;
        GameObject backSlotM = slotM.GetComponent<W94_Slot>().backSlot;
        GameObject backSlotR = slotR.GetComponent<W94_Slot>().backSlot;

        if (backSlotL.transform.childCount > 0)
        {
            GameObject bookInBack = backSlotL.transform.GetChild(0).gameObject;
            bookInBack.transform.SetParent(slotL.transform);
            bookInBack.GetComponent<RectTransform>().position = slotL.GetComponent<RectTransform>().position;
            bookInBack.GetComponent<CanvasGroup>().interactable = true;
            bookInBack.GetComponent<Image>().DOColor(Color.white, fadeTime);
        }

        if (backSlotM.transform.childCount > 0)
        {
            GameObject bookInBack = backSlotM.transform.GetChild(0).gameObject;
            bookInBack.transform.SetParent(slotM.transform);
            bookInBack.GetComponent<RectTransform>().position = slotM.GetComponent<RectTransform>().position;
            bookInBack.GetComponent<CanvasGroup>().interactable = true;
            bookInBack.GetComponent<Image>().DOColor(Color.white, fadeTime);
        }

        if (backSlotR.transform.childCount > 0)
        {
            GameObject bookInBack = backSlotR.transform.GetChild(0).gameObject;
            bookInBack.transform.SetParent(slotR.transform);
            bookInBack.GetComponent<RectTransform>().position = slotR.GetComponent<RectTransform>().position;
            bookInBack.GetComponent<CanvasGroup>().interactable = true;
            bookInBack.GetComponent<Image>().DOColor(Color.white, fadeTime);
        }

        W94_GameManager.instance.CheckStuck();
    }

    public void PlayParticles()
    {
        outlineParticle.Play();
        glowParticle.Play();
    }
}
