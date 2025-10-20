using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class W14_GridManager : MonoBehaviour
{
    // FIELD
    private readonly int _cols = 5;
    private readonly int _rows = 7;

    public List<W14_Tile> _tiles;

    [SerializeField] private W14_Tile _refTile;
    [SerializeField] private W14_GameManager _gameManager;
    [SerializeField] private W14_UIManager _uiManager;
    [SerializeField] Transform _tileStartPos;
    public GameObject tileParent;

    // MAIN
    void OnEnable()
    {
        _gameManager.onStartGameEvent += TileGenerator;
    }

    // FUNC
    /// <summary>
    /// Creates Tile-Grid
    /// </summary>
    private void TileGenerator()
    {
        _tiles = new List<W14_Tile>();
        Vector3 v3 = new Vector3(_tileStartPos.position.x, _tileStartPos.position.y, 1f);
        float x = _tileStartPos.position.x;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                W14_Tile tile = Instantiate(_refTile, v3, Quaternion.identity, tileParent.transform);
                tile._gameManager = _gameManager;
                tile._uiManager = _uiManager;
                _tiles.Add(tile);
                v3.x += 0.75f;
            }

            v3.y -= 0.75f;
            v3.x = x;
        }
    }
}