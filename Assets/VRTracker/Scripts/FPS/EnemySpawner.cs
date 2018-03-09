using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class to handle the different information on a enemy to be spawned
/// </summary>
[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int level;
    public int spawnTime;
}

/// <summary>
/// Spawn Point will hold the information of a spawnPoint
/// </summary>
[System.Serializable]
public class SpawnPoint
{
    public bool enabled = false;
    public GameObject spawPoint;
}

/// <summary>
/// EnemySpawner handle the different waves of enemy and the spawn rate
/// It is currenlty handled by the network server
/// </summary>
public class EnemySpawner : NetworkBehaviour
{

    public Enemy[] enemies;//array of enemies
    public List<SpawnPoint> spawnPoints; //List of spawnpoint
    public bool isSpawning = false;

    private bool isWaiting = false;
    private float spawnRate = 5f;

    private int zombieIndex = 0; //number of current spawned zombies
    private int currentWave = 0; //index of current wave
    private PlayerHealth playerHealth;       // Reference to the player's heatlh.
    private List<GameObject> enemyList; //List containing all the spawned enemies


    public void Start()
    {
        enemyList = new List<GameObject>();
    }

    public void SetSpawnRate(float newRate)
    {
        spawnRate = newRate;
    }

    private void Spawn(int currentWave = 0)
    {
        if (this != null)
        {
            Debug.Log("enemylist " + enemyList.Count);
            Debug.Log("Wave number " + currentWave);

            //Check if there is still a new wave
            if (enemyList.Count < WaveManager.instance.waveList[currentWave].quantity)
            {
                // Find a random index between zero and one less than the number of spawn points.
                int spawnPointIndex = Random.Range(0, spawnPoints.Count);
                int enemyIndex = Random.Range(0, currentWave);

                //Create new enemy game object
                var enemy = (GameObject)Instantiate(enemies[enemyIndex].enemyPrefab, spawnPoints[spawnPointIndex].spawPoint.transform.position, spawnPoints[spawnPointIndex].spawPoint.transform.rotation);

                enemyList.Add(enemy);
                //Spawn it on the network
                NetworkServer.Spawn(enemy);
            }
            
           
        }
    }

    /// <summary>
    /// Spawn wave will spawn a new enmy by calling the function Spawn
    /// Going to be called until all the enmies for this wave are spawned
    /// </summary>
    /// <param name="enemyNumber">enemy left to be spawned</param>
    public void SpawnWave(int enemyNumber)
    {
        if (enemyNumber > 0)
        {
            isSpawning = true;
            if (isServer)
            {
                Spawn(currentWave);
            }
            else
            {
                Debug.Log("Not server");

            }
            StartCoroutine(WaitForSpawn(enemyNumber));
        }
        else
        {
            isSpawning = false;
        }
    }

    /// <summary>
    /// WaitForSpawn, will spawn an enemy at spawnrate spawnRate
    /// </summary>
    /// <param name="number">Number of enemy left to be spawned</param>
    /// <returns></returns>
    IEnumerator WaitForSpawn(int number)
    {
        yield return new WaitForSeconds(spawnRate);
        SpawnWave(--number);
    }

    public void SetWave(int waveNumber)
    {
        currentWave = waveNumber;
    }
    
    /// <summary>
    /// Clear and destroyed all the enemies 
    /// </summary>
    public void ClearEnemies()
    {
        if(enemyList != null)
        {
            foreach (GameObject enemy in enemyList)
            {
                if (enemy != null)
                {
                    NetworkServer.Destroy(enemy);
                }
            }
            enemyList.Clear();
        }

    }

    /// <summary>
    /// Stop wave will stop the spawn and destroy all the enemies
    /// </summary>
    public void StopWaves()
    {
        isSpawning = false;
        ClearEnemies();
    }

    public int GetEnemyCount()
    {
        return enemyList.Count;
    }

}
