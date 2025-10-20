using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class ManualGridLayout : MonoBehaviour
    {
        private GridLayoutGroup _grid;

        public void ArrangeGrid()
        {
            _grid = GetComponent<GridLayoutGroup>();

            switch (LevelManager.instance.cardAmount)
            {
                case 4:
                    _grid.cellSize = new Vector2(375f, 471f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 2;
                    break;

                case 6:
                    _grid.cellSize = new Vector2(375f, 471f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 2;
                    break;

                case 8:
                    _grid.cellSize = new Vector2(300f, 377f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 2;
                    break;

                case 10:
                    _grid.cellSize = new Vector2(231f, 290f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 2;
                    break;

                case 12:
                    _grid.cellSize = new Vector2(250f, 314f);
                    _grid.spacing = new Vector2(25f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 3;
                    break;

                case 14:
                    _grid.cellSize = new Vector2(231f, 290f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 3;
                    break;

                case 16:
                    _grid.cellSize = new Vector2(188f, 236f);
                    _grid.spacing = new Vector2(25f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 4;
                    break;

                case 18:
                    _grid.cellSize = new Vector2(188f, 236f);
                    _grid.spacing = new Vector2(50f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 3;
                    break;

                case 20:
                    _grid.cellSize = new Vector2(188f, 236f);
                    _grid.spacing = new Vector2(25f, 50f);
                    _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _grid.constraintCount = 4;
                    break;
            }
        }
    }
}