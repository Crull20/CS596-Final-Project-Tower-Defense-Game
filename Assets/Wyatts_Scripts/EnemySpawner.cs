using UnityEngine;

/*
This class is responsible for spawning enemy game objects at regular intervals. 
It uses a timer to determine when to spawn the next enemy and instantiates a new 
enemy prefab at the position of the spawner when the timer reaches zero.
*/

public class EnemySpawner : MonoBehaviour
{
    private float spawnTimer = 0f;
    [SerializeField] private float spawnInterval = 1f;

    [SerializeField] private ObjectPooler pool;
  
    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0){
            //reset the spawn timer
            spawnTimer = spawnInterval;
            //spawn an enemy at the position of the spawner
            SpawnEnemy();
        }
    }

    private void SpawnEnemy(){
        //instantiate an enemy prefab at the position of the spawner 
        GameObject spawnedEnemy = pool.GetPooledObject();  
        spawnedEnemy.transform.position = transform.position;
        spawnedEnemy.SetActive(true);
    }
}
