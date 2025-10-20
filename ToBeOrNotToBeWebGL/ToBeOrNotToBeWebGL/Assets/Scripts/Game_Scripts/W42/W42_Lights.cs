using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W42_Lights : MonoBehaviour
{
    [SerializeField] private float timeInterval;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private List<Sprite> lightSprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Light(bool choice)
    {
        if (choice)
            StartCoroutine(LightRoutine());
        else
        {
            StopAllCoroutines();
            spriteRenderer.sprite = offSprite;
        }
    }

    IEnumerator LightRoutine()
    {
        spriteRenderer.sprite = lightSprites[0];
        yield return new WaitForSeconds(timeInterval);
        spriteRenderer.sprite = lightSprites[1];
        yield return new WaitForSeconds(timeInterval);
        yield return LightRoutine();
    }
}
