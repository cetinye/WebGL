using UnityEngine;

public class W27_Block : MonoBehaviour
{
    public GameObject LeftWall, RightWall, UpWall, DownWall;
    public Vector2 position;
    
    void Start()
    {
        transform.position = position;
    }
}