using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CoinCounter coinCounter;

    public Path pathToAssign;
    private List<GameObject> pool;

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(prefab, transform);

        Enemy enemy = newObj.GetComponent<Enemy>();
        enemy.currentPath = pathToAssign;
        enemy.SetHealthBar(healthBar);

        if (coinCounter != null)
        {
            enemy.OnDeath -= coinCounter.OnDeath;
            enemy.OnDeath += coinCounter.OnDeath;
        }

        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                enemy.currentPath = pathToAssign;
                enemy.SetHealthBar(healthBar);

                if (coinCounter != null)
                {
                    enemy.OnDeath -= coinCounter.OnDeath;
                    enemy.OnDeath += coinCounter.OnDeath;
                }

                return obj;
            }
        }

        return CreateNewObject();
    }
}