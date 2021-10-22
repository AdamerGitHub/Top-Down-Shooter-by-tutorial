using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;
    public LivingEntity[] enemy;

    LivingEntity playerEntity;
    Transform playerT;
    public Material originalTileMat;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    GameObject shopGameObject;

    public event System.Action<int> OnNewWave;

    void Awake()
    {
        shopGameObject = GameObject.FindGameObjectWithTag("ShopUI");
    }

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
        //shopGameObject.SetActive(false);
    }

    void Update()
    {
        if (!isDisabled && !shopGameObject.activeSelf)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        /*if (isCamping)
        {
            spawnTile = map.GetTileFromPositon(playerT.position);
        }*/
        Material tileMat = spawnTile.GetComponent<Renderer>().material;

        Color initialColour = originalTileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.SetColor("_BaseColor", Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1)));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.SetColor("_BaseColor", initialColour);

        LivingEntity spawnedEnemy = Instantiate(enemy[2], spawnTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPositon(Vector3.zero).position + Vector3.up * 2;
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                shopGameObject.SetActive(true);
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }
    
    [System.Serializable]
    public class Wave
    {
        //public EnemyInWave[] enemyInWave;

        [Header("Spawn Settings")]
        public int enemyCount;
        public float timeBetweenSpawn;
        public bool infinite;

        [Header("Spawn Once Settings")]
        public int enemyRemainToSpawnOnce;

        public int enemyCountToSpawnOnce;
        public float timeBetweenSpawnOnce;
    }
}
