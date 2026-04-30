using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Serializable]
    public struct Wave
    {
        public int enemyCount;
        public float spawnInterval;
    }

    [Header("Pools & Prefabs")]
    public ObjectPooler regularPool;
    public GameObject bossPrefab;

    [Header("Spawning")]
    public Transform spawnPoint;
    public Path pathToAssign;
    public float pauseBetweenWaves = 10f;

    //hardcoded wave definitions — edit counts and intervals here
    private readonly Wave[] waves = new Wave[]
    {
        new Wave { enemyCount = 5,  spawnInterval = 1.0f }, //wave 1
        new Wave { enemyCount = 8,  spawnInterval = 1.0f }, //wave 2
        new Wave { enemyCount = 10, spawnInterval = 0.9f }, //wave 3
        new Wave { enemyCount = 12, spawnInterval = 0.8f }, //wave 4
        new Wave { enemyCount = 15, spawnInterval = 0.7f }, //wave 5 — boss wave
        new Wave { enemyCount = 18, spawnInterval = 0.7f }, //wave 6
        new Wave { enemyCount = 20, spawnInterval = 0.6f }, //wave 7
        new Wave { enemyCount = 22, spawnInterval = 0.6f }, //wave 8
        new Wave { enemyCount = 25, spawnInterval = 0.5f }, //wave 9
        new Wave { enemyCount = 30, spawnInterval = 0.5f }, //wave 10 — boss wave, final wave
    };

    //ui events
    public event Action<int> OnWaveStart;
    public event Action<float> OnPauseTick;
    public event Action<int> OnEnemyCountChanged;
    public event Action OnAllWavesComplete;

    private int activeEnemyCount = 0;

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        //initial pause before the first wave so the player can get ready
        yield return StartCoroutine(PauseCountdown(pauseBetweenWaves));

        for (int i = 0; i < waves.Length; i++)
        {
            int waveNumber = i + 1; //convert to 1-indexed for display and boss check
            Wave wave = waves[i];

            //tell listeners the wave number so they can update the ui
            OnWaveStart?.Invoke(waveNumber);

            //spawn regular enemies first
            for (int j = 0; j < wave.enemyCount; j++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            //spawn boss last so it trails behind the regular enemies
            if (waveNumber % 5 == 0 || waveNumber == 1)
                SpawnBoss();

            //hold here until every enemy and boss from this wave is gone
            yield return new WaitUntil(() => activeEnemyCount <= 0);

            //all waves cleared — fire the complete event and stop
            if (waveNumber == waves.Length)
            {
                OnAllWavesComplete?.Invoke();
                yield break;
            }

            //count down the break before the next wave
            yield return StartCoroutine(PauseCountdown(pauseBetweenWaves));
        }
    }

    private void SpawnEnemy()
    {
        //pull an inactive object from the pool — pool already assigns currentPath
        GameObject obj = regularPool.GetPooledObject();
        obj.transform.position = spawnPoint != null ? spawnPoint.position : transform.position;

        //subscribe before activating so we don't miss the disable event
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
            enemy.BecameUnavailable += OnEnemyUnavailable;

        activeEnemyCount++;
        OnEnemyCountChanged?.Invoke(activeEnemyCount);

        obj.SetActive(true);
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;

        //instantiate boss directly since it is not managed by the pool
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject bossObj = Instantiate(bossPrefab, pos, Quaternion.identity);
        bossObj.SetActive(false); //disable immediately so OnEnable doesn't fire before path is set

        //assign path and hook the unavailable event while the boss is still inactive
        Enemy bossEnemy = bossObj.GetComponent<Enemy>();
        if (bossEnemy != null)
        {
            bossEnemy.currentPath = pathToAssign;
            bossEnemy.BecameUnavailable += OnEnemyUnavailable;
        }

        activeEnemyCount++;
        OnEnemyCountChanged?.Invoke(activeEnemyCount);

        //activate last so OnEnable runs with currentPath already assigned
        bossObj.SetActive(true);
    }

    private void OnEnemyUnavailable(Enemy enemy)
    {
        //unsubscribe immediately so recycled pool objects don't double-fire
        enemy.BecameUnavailable -= OnEnemyUnavailable;

        activeEnemyCount--;
        if (activeEnemyCount < 0) activeEnemyCount = 0; //safety clamp

        OnEnemyCountChanged?.Invoke(activeEnemyCount);
    }

    private IEnumerator PauseCountdown(float duration)
    {
        //fire every frame so ui can display a smooth countdown
        float remaining = duration;
        while (remaining > 0f)
        {
            OnPauseTick?.Invoke(remaining);
            yield return null;
            remaining -= Time.deltaTime;
        }
        OnPauseTick?.Invoke(0f);
    }
}
