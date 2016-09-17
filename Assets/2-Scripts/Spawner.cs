using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Panda;

public class Spawner : MonoBehaviour
{
    public LayerMask obstacleMask;

    public Wave[] waves;
    public Enemy enemy;

    Player playerEntity;
    Transform playerT;
    GameUI gameUI;

    Wave currentWave;
    int currentWaveNumber = 0;

    int enemiesRemainingToSpawn;
    float nextSpawnTime = 0f;

    int enemiesRemainingAlive;
    private MapGenerator mapGen;
    private Map map;

    public event System.Action<int> OnNewWave;

    private bool startSpawningRopeEnemies;

    void Start()
    {
        PoolManager.instance.CreatePool(enemy.gameObject, 20);
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.GetComponent<Transform>();

        playerEntity.OnDeath += OnPlayerDeath;
        //
        NextWave();

        mapGen = FindObjectOfType<MapGenerator>();
        map = mapGen.map;

        SpawnPatrolEnemes();
        //
        gameUI = FindObjectOfType<GameUI>();
    }

    void Update()
    {
        if (startSpawningRopeEnemies && (enemiesRemainingToSpawn > 0 || currentWave.infinity) && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            StartCoroutine(SpawnEnemyFromRope());
        }
    }

    void SpawnPatrolEnemes()
    {
        SpawnRect[] quadrants = new SpawnRect[4];
        float patrolY = (3f / 4f) * map.mapSize.y;
        quadrants[0] = new SpawnRect(map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4, map.mapSize.x / 2, patrolY / 2, transform);
        quadrants[1] = new SpawnRect(-map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4, map.mapSize.x / 2, patrolY / 2, transform);
        quadrants[2] = new SpawnRect(map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4 - patrolY / 2, map.mapSize.x / 2, patrolY / 2, transform);
        quadrants[3] = new SpawnRect(-map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4 - patrolY / 2, map.mapSize.x / 2, patrolY / 2, transform);

        int[] enemisCount = new int[4];
        enemisCount[0] = enemisCount[1] = enemisCount[2] = currentWave.initialPatrolEnemies / 4;
        enemisCount[3] = currentWave.initialPatrolEnemies - 3 * enemisCount[0];
        for (int i = 0; i < quadrants.Length; i++)
        {
            for (int j = 0; j < enemisCount[i]; j++)
            {
                Vector3 spawnPos = mapGen.GetSpawnPositionInRect(quadrants[i], 1) + Vector3.up;
                Transform enemyT = PoolManager.instance.ReuseObject(enemy.gameObject, spawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f),0f)).transform;
                enemyT.GetComponent<Enemy>().OnDeath += OnEnemyDeath;
                enemiesRemainingToSpawn--;
            }
        }
    }

    IEnumerator SpawnEnemyFromRope()
    {
        yield return new WaitForSeconds(1f);
        SpawnRect spawnRect = playerEntity.spawnRect;
        spawnRect.rect.x += playerEntity.moveVelocity.x;
        spawnRect.rect.y += playerEntity.moveVelocity.z;

        Vector3 spawnPos = mapGen.GetSpawnPositionInRect(spawnRect) + Vector3.up;
        Vector3 startSpawnPos = spawnPos + Vector3.up * 20f;
        Transform enemyT = PoolManager.instance.ReuseObject(enemy.gameObject, startSpawnPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).transform;
        Transform rope = ((Rope)Instantiate(enemyT.GetComponent<Enemy>().rope, startSpawnPos, Quaternion.identity)).transform;

        PandaBehaviour panda = enemyT.GetComponent<PandaBehaviour>();
        panda.enabled = false;

        float percent = 0f;
        float ropeVelocity = 1 / 0.3f;
        while (percent <1)
        {
            percent += ropeVelocity * Time.deltaTime;
            rope.position = Vector3.Lerp(startSpawnPos, spawnPos, percent);
            yield return null;
        }

        percent = 0;
        float climbDownVel = 1 / 1.5f;
        while (percent < 1)
        {
            percent += climbDownVel * Time.deltaTime;
            enemyT.position = Vector3.Lerp(startSpawnPos, spawnPos, percent);
            yield return null;
        }
        enemyT.GetComponent<Enemy>().OnDeath += OnEnemyDeath;
        enemyT.position = spawnPos;
        panda.enabled = true;
        enemyT.GetComponent<Enemy>().StartChase();
        Destroy(rope.gameObject, 2);

    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if (!startSpawningRopeEnemies && enemiesRemainingAlive - currentWave.enemiesSpawnOnPlay <= currentWave.initialPatrolEnemies/2)
            startSpawningRopeEnemies = true;

        if (enemiesRemainingAlive == 0)
            NextWave();
    }

    void OnPlayerDeath()
    {
        StopAllCoroutines();
    }

    void NextWave()
    {
        if (currentWaveNumber > 0)
            AudioManager.instance.PlaySound2D("Level Complete");

        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
                OnNewWave(currentWaveNumber);
            //ResetPlayerPosition();
        }
        else
        {
            gameUI.OnWin();
        }
    }

    void OnDrawGizmos()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        map = mapGen.map;
        float patrolY = (3f / 4f) * map.mapSize.y;
        SpawnRect topRightRect = new SpawnRect(map.mapSize.x / 4, map.mapSize.y/2 - patrolY/4, map.mapSize.x/2, patrolY / 2, transform);
        SpawnRect topLeftRect = new SpawnRect(-map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4, map.mapSize.x / 2, patrolY / 2, transform);
        SpawnRect bottomRightRect = new SpawnRect(map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4 - patrolY/2, map.mapSize.x / 2, patrolY / 2, transform);
        SpawnRect bottomLeftRect = new SpawnRect(-map.mapSize.x / 4, map.mapSize.y / 2 - patrolY / 4 - patrolY / 2, map.mapSize.x / 2, patrolY / 2, transform);

        //topRightRect.DrawRectangle();
        //topLeftRect.DrawRectangle();
        bottomRightRect.DrawRectangle();
        //bottomLeftRect.DrawRectangle();
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount { get { return initialPatrolEnemies + enemiesSpawnOnPlay; } }
        public bool infinity;
        public int initialPatrolEnemies;
        public int enemiesSpawnOnPlay;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}