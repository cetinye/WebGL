using UnityEngine;

namespace W3_Scripts
{
    public class W3_Level
    {
        // FIELD
        public int ShapeCount { get; set; }

        public int WrongShapeCount { get; set; }

        public Vector2 ShapeCardSize { get; set; }

        public float ShapeCardYDistance { get; set; }

        public Vector3 ShapeCardStartPos { get; set; }

        public Vector3 ShapeCardMidPos { get; set; }

        public float MoveSpeed { get; set; }

        // CTOR
        public W3_Level(int shapeCount, int wrongShapeCount, Vector2 shapeCardSize, float shapeCardDistance,
            Vector3 shapeCardStartPos, Vector3 shapeCardMidPos, float moveSpeed)
        {
            ShapeCount = shapeCount;
            WrongShapeCount = wrongShapeCount;
            ShapeCardSize = shapeCardSize;
            ShapeCardStartPos = shapeCardStartPos;
            ShapeCardMidPos = shapeCardMidPos;
            ShapeCardYDistance = shapeCardDistance;
            MoveSpeed = moveSpeed;
        }
    }
}