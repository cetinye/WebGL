using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class W59_Pedestrian : MonoBehaviour
{
    [SerializeField] private float startDelay;
    [SerializeField] private List<Sprite> spriteList = new List<Sprite>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float spriteSwapInterval;
    [SerializeField] private Transform rightEdge, leftEdge;
    [SerializeField] private float timeToMove;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartMovement", startDelay);
    }

    void StartMovement()
    {
        StartCoroutine(ChangeSprite());
        Move();
    }

    void Move()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        if (transform.localScale.x > 0)
            transform.DOMove(leftEdge.position, timeToMove).SetEase(Ease.Linear).OnComplete(() => Move());

        else
            transform.DOMove(rightEdge.position, timeToMove).SetEase(Ease.Linear).OnComplete(() => Move());
    }

    IEnumerator ChangeSprite()
    {
        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteRenderer.sprite = spriteList[i];
            yield return new WaitForSeconds(spriteSwapInterval);
        }

        yield return ChangeSprite();
    }
}
