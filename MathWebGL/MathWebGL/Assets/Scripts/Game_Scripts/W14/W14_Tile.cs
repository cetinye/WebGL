using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Witmina_Math;

public class W14_Tile : MonoBehaviour, IPointerClickHandler
{
    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        ChooseNumber();
    }

    // FIELD
    public W14_UIManager _uiManager;
    public W14_GameManager _gameManager;
    public TextMeshPro txtTileNumber;
    public Sequence _sequence;

    public bool _isEnable = true;

    // FUNC
    /// <summary>
    /// Send number to the operation panel by mouse click.
    /// </summary>
    private void ChooseNumber()
    {
        if (txtTileNumber.text == "" || !_isEnable || !_gameManager._isTimersOn) return;

        if (_uiManager.txtNumber.text == "....")
        {
            _isEnable = false;
            _uiManager.txtNumber.text = txtTileNumber.text;
            _gameManager.selectedTiles.Add(this);
            W14_AudioManager.instance.PlayOneShot("NumberTap");
            Taptic.Light();
            TileSeq(_uiManager.txtNumber, false);
        }
        else
        {
            if (_uiManager.txtNumber2.text != "....") return;
            _isEnable = false;
            _uiManager.txtNumber2.text = txtTileNumber.text;
            _gameManager.selectedTiles.Add(this);
            W14_AudioManager.instance.PlayOneShot("NumberTap");
            Taptic.Light();
            TileSeq(_uiManager.txtNumber2, false);
        }

        _sequence = TileSeq(txtTileNumber, true);
        _sequence.onComplete = () => txtTileNumber.gameObject.SetActive(false);
        _sequence.SetAutoKill();

        if (_uiManager.txtNumber.text != "...." && _uiManager.txtNumber2.text != "....")
        {
            _gameManager.Submit();
        }
    }

    /// <summary>
    /// Scale animation works when choosing a number.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isTileNumber">If it's not it is for textNumbers on operation panel.</param>
    /// <returns></returns>
    public Sequence TileSeq(TMP_Text text, bool isTileNumber)
    {
        Tweener tween, tween2;
        float duration;
        Color color = _uiManager.resultColor,
            color2 = _uiManager.numberColor;
        color.a = 100;
        color2.a = 100;

        if (isTileNumber)
        {
            duration = 0.5f;
            tween = text.DOScale(1.5f, duration);
            tween2 = text.DOScale(1f, 0f);
            //tween2 = text.DOScale(1f, duration);
        }
        else
        {
            duration = 0.3f;
            tween = text.DOScale(1.2f, duration);
            tween2 = text.DOScale(1f, duration);
        }

        if (isTileNumber)
        {
            return DOTween.Sequence()
            .Append(tween)
            .Join(text.DOFade(0f, duration))
            .Append(tween2)
            .Append(text.DOColor(new Color(237f, 204f, 150f, 1f), 0))
            .OnComplete(() =>
            {
                _uiManager.txtNumber.color = color2;
                _uiManager.txtNumber2.color = color2;
            });
        }
        else
        {
            return DOTween.Sequence()
            .Append(tween)
            .Append(tween2)
            .Append(text.DOColor(new Color(237f, 204f, 150f, 1f), 0))
            .OnComplete(() =>
            {
                _uiManager.txtNumber.color = color2;
                _uiManager.txtNumber2.color = color2;
            });
        }

    }
}