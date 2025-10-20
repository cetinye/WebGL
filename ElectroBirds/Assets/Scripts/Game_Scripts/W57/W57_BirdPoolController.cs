using System.Collections.Generic;
using UnityEngine;

public class W57_BirdPoolController : MonoBehaviour
{
    [SerializeField] private W57_Bird birdInstance;
    [SerializeField] private W57_LevelManager levelManager;

    [SerializeField] private List<W57_Bird> objectPool = new List<W57_Bird>();
    [SerializeField] private List<W57_Bird> activeBirds = new List<W57_Bird>();
    private static int objectPoolSize = 10;

    private void Start()
    {
        CreateObjectPool();
    }

    private void CreateObjectPool()
    {
        for (var i = 0; i < objectPoolSize; i++)
        {
            W57_Bird obj = Instantiate(birdInstance, new Vector3(-50, 50, 100), Quaternion.identity, transform);
            obj.gameObject.SetActive(false);
            obj.levelManager = levelManager;
            objectPool.Add(obj);
        }
    }

    public W57_Bird GetObjectFromPool()
    {
        foreach (W57_Bird obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.transform.position = new Vector3(-50, 50, 100);
                activeBirds.Add(obj);
                return obj;
            }
        }

        W57_Bird newObj = Instantiate(birdInstance, new Vector3(-50, 50, 100), Quaternion.identity, transform);
        newObj.gameObject.SetActive(false);
        newObj.levelManager = levelManager;
        objectPool.Add(newObj);

        return newObj;
    }

    public void SetAllInactive()
    {
        foreach (var bird in objectPool)
        {
            bird.gameObject.SetActive(false);
        }
    }

    public W57_Bird GetRandomBird()
    {
        W57_Bird bird = activeBirds[Random.Range(0, activeBirds.Count)];
        activeBirds.Remove(bird);
        return bird;
    }
}