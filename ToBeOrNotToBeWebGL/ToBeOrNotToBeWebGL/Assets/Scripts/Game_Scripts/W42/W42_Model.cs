using DG.Tweening;
using Lean.Localization;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class W42_Model : MonoBehaviour
{
    public W42_GameController gameController;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public SpriteRenderer baseSpr;
    public SpriteRenderer spriteSpr;
    public int oldSpriteOrder;
    public Transform tr;

    public string _modelName;
    public string colorName;
    void Awake()
    {
        tr = transform;
    }
    void Start()
    {
        startPosition = tr.position;
        startRotation = tr.rotation;

        Color color = spriteSpr.color;
        color.a = 0;
        spriteSpr.color = color;

        tr.localScale = new Vector3(0.22f, 0.22f, 0.22f);
    }

    // FUNC
    public void SetModel(string name, Sprite sprite)
    {
        SetNameNSprite(name, sprite);
        SetColor();
    }
    private void SetColor()
    {
        spriteSpr.color = gameController.valueList[Random.Range(0, gameController.valueList.Count)];
        colorName = (from item in gameController.colorsWithName
                     where item.Value.Equals(spriteSpr.color)
                     select item.Key)
            .FirstOrDefault();
        colorName = LeanLocalization.GetTranslationText(colorName);
    }
    private void SetNameNSprite(string name, Sprite sprite)
    {
        this._modelName = name;
        spriteSpr.sprite = sprite;
        tr.localScale = new Vector3(0.22f, 0.22f, 0);
    }
    public void ShowTheModel()
    {
        DOTween.Sequence()
            .Append(baseSpr.DOFade(1, 2))
            .Join(spriteSpr.DOFade(1, 2))
            .SetAutoKill(true);
    }
    public void Shaking()
    {
        DOTween.Sequence()
            .Append(tr.DOShakePosition(1))
            .SetAutoKill(true);
    }
}