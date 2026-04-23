using System.Collections.Generic;
using UnityEngine;
/*
This class is responsible for managing a pool of reusable game objects, allowing for efficient
spawning and despawning of objects without the overhead of instantiating and destroying them repeatedly.

The idea here is that when we need a new enemy, we activate an inactive enemy from the pool instead of
creating a new one. When an enemy is no longer needed (e.g., when it reaches the end of the path or is
destroyed), we can simply deactivate it and return it to the pool for future use. This approach can help
improve performance by reducing the number of instantiations and destructions, which can be costly operations
in Unity.
*/
public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    public Path pathToAssign;
    private List<GameObject> pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //initialize the pool list and populate it with inactive instances of the prefab
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        //instantiate a new instance of the prefab, set it to inactive, add it to the pool list, and return it
        GameObject newObj = Instantiate(prefab, transform);
        newObj.GetComponent<Enemy>().currentPath = pathToAssign;
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    public GameObject GetPooledObject()
    {
        //search the pool for an inactive object, return it if found, otherwise create a new one and return it
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.GetComponent<Enemy>().currentPath = pathToAssign;
                return obj;
            }
        }
        return CreateNewObject();
    }

}


