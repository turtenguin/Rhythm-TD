using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public struct enemySpawn
    {
        public enemySpawn(int type, float time)
        {
            enemyType = type;
            spawnTime = time;
        }

        public int enemyType { get; private set; }
        public float spawnTime { get; private set; }
    }

    private Queue<enemySpawn> spawnList;
    public Enemy[] enemyPrefabs;
    public float sortTime = .2f;
    public List<Enemy> enemies { get; private set; }
    private Queue<Enemy>[] pooledEnemies;

    private void Awake()
    {
        enemies = new List<Enemy>();
        pooledEnemies = new Queue<Enemy>[enemyPrefabs.Length];
        for(int i = 0; i < pooledEnemies.Length; i++)
        {
            pooledEnemies[i] = new Queue<Enemy>();
        }
    }

    private void Start()
    {
        Invoke("RoutineSortEnemies", sortTime);
    }

    public void LoadEnemyData(Queue<enemySpawn> newSpawnList)
    {
        spawnList = newSpawnList;
    }

    public void RunSpawner()
    {
        Invoke("Spawn", spawnList.Peek().spawnTime);
    }

    public void Spawn()
    {
        enemySpawn spawn = spawnList.Dequeue();
        int type = spawn.enemyType;

        if(pooledEnemies[type].Count == 0)
        {
            Enemy newEnemy = Object.Instantiate(enemyPrefabs[type]);
            newEnemy.type = type;
        } else
        {
            Enemy newEnemy = pooledEnemies[type].Dequeue();
            newEnemy.gameObject.SetActive(true);
            newEnemy.ResetEnemy();
        }

        if(spawnList.Count > 0)
        {
            Invoke("Spawn", spawnList.Peek().spawnTime);
        }
    }

    public void AddToPool(Enemy enemy)
    {
        pooledEnemies[enemy.type].Enqueue(enemy);
    }

    //Sort using insertion sort with swaps. Since the most common scenario is switching two adjacent entries, this should take about O(n) time.
    public void SortEnemies()
    {
        for(int i = 1; i < enemies.Count; i++)
        {
            for(int j = i; j > 0; j--)
            {
                if(enemies[j].IsAheadOf(enemies[j - 1]))
                {
                    Swap<Enemy>(enemies, j, j - 1);
                } else
                {
                    break;
                }
            }
        }
    }

    private static void Swap<T>(List<T> list, int i1, int i2)
    {
        T first = list[i1];
        list[i1] = list[i2];
        list[i2] = first;
    }

    private void RoutineSortEnemies()
    {
        SortEnemies();
        Invoke("RoutineSortEnemies", sortTime);
    }
}
