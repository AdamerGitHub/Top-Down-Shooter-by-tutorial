using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKing_SpawnEnemies : MonoBehaviour
{
    public int enemyCount = 3;
    public List<Enemy> enemiesList;
    public float spawnDistance = 2f;

    public float spawnEvery = 2;

    public Enemy enemyPref; // enemyPrefab
    public Enemy enemyKing;

    // Start is called before the first frame update
    void Start()
    {
        enemiesList = new List<Enemy>();
        enemyKing = GetComponent<Enemy>();
        StartCoroutine(SpawnEnemyArmy());

        enemyKing.OnDeath += OnKingDie;
    }

    void OnKingDie()
    {
        ClearDestroyedEnemies();

        // Remove all enemies
        foreach (Enemy enemy in enemiesList)
        {
            enemy.TakeDamage(9999, 0);
        }
    }

    void ClearDestroyedEnemies()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (enemiesList[i] == null)
            {
                enemiesList.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update()
    {
        
    }

    void SpawnEnemy(Vector3 dir)
    {
        RaycastHit raycastHit;
        int layerMask = 1 << 10; // Obstacle layerMask

        if (!Physics.Raycast(transform.position, dir, out raycastHit, spawnDistance, layerMask))
        {
            raycastHit.point = transform.position + dir * spawnDistance;
        }

        Debug.DrawLine(transform.position, raycastHit.point, Color.white, 10f);

        // If didn't hit any obstacle in layer Default (that means he exclude all Enemies, Player an so on, but include all obstacles)
        Enemy spawnedEnemy = Instantiate(enemyPref, raycastHit.point + Vector3.up, Quaternion.identity);
        spawnedEnemy.transform.forward = transform.forward;
        spawnedEnemy.goldOnDeath = 0; // Don't give gold on death
        enemiesList.Add(spawnedEnemy);
    }

    IEnumerator SpawnEnemyArmy()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnEvery);

            ClearDestroyedEnemies();

            // Right
            if (enemyCount > enemiesList.Count)
            {
                SpawnEnemy(transform.right);
            }

            // Left
            if (enemyCount > enemiesList.Count)
            {
                SpawnEnemy(-transform.right);
            }

            // Forward
            if (enemyCount > enemiesList.Count)
            {
                SpawnEnemy(transform.forward);
            }
        }
    }
}
