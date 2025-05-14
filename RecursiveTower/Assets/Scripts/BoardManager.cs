using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    [Header("Board Settings")]
    public int columns = 8;
    public int rows = 8;
    public float tileSize = 16;

    [Header("Tiles")]
    public GameObject[] floorTiles;
    public GameObject[] enemyPrefabs;

	[Header("Outer Wall Tiles")]
	public GameObject[] topWallTiles;
	public GameObject[] bottomWallTiles;
	public GameObject[] leftWallTiles;
	public GameObject[] rightWallTiles;

	[Header("Corner Wall Tiles (specific)")]
	public GameObject cornerTopLeft;
	public GameObject cornerTopRight;
	public GameObject cornerBottomLeft;
	public GameObject cornerBottomRight;

    [Header("Enemy Spawning")]
    public int initialEnemyCount = 4;
    public float initialSpawnDelay = 5f;
    public float spawnAcceleration = 0.95f; // Must be < 1 to accelerate
    public float minSpawnDelay = 1.5f;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    private float currentSpawnDelay;
    private Coroutine spawnRoutine;

    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 0 - (int)tileSize / 2; x <= (columns - 1) * tileSize + (int)tileSize / 2; x++)
        {
            for (int y = 0 - (int)tileSize / 2; y <= (rows - 1) * tileSize + (int)tileSize / 2; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        // Remove previous board if it exists
        Transform oldBoard = transform.Find("Board");
        if (oldBoard != null)
            Destroy(oldBoard.gameObject);

        boardHolder = new GameObject("Board").transform;
        boardHolder.SetParent(transform);

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

				// Specific corners
				if (x == -1 && y == -1)
				{
					toInstantiate = cornerBottomLeft;
				}
				else if (x == columns && y == -1)
				{
					toInstantiate = cornerBottomRight;
				}
				else if (x == -1 && y == rows)
				{
					toInstantiate = cornerTopLeft;
				}
				else if (x == columns && y == rows)
				{
					toInstantiate = cornerTopRight;
				}
				else if (x == -1)
				{
					toInstantiate = leftWallTiles[Random.Range(0, leftWallTiles.Length)];
				}
				else if (x == columns)
				{
					toInstantiate = rightWallTiles[Random.Range(0, rightWallTiles.Length)];
				}
				else if (y == -1)
				{
					toInstantiate = bottomWallTiles[Random.Range(0, bottomWallTiles.Length)];
				}
				else if (y == rows)
				{
					toInstantiate = topWallTiles[Random.Range(0, topWallTiles.Length)];
				}


                GameObject instance = Instantiate(toInstantiate, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

	void SpawnEnemyAtEdge()
	{
		List<Vector3> spawnPositions = new List<Vector3>();

		// One tile inward from the bottom and top edges
		for (int x = 0; x < columns; x++)
		{
			spawnPositions.Add(new Vector3(x * tileSize, 1 * tileSize, 0f));               // just above bottom wall
			spawnPositions.Add(new Vector3(x * tileSize, (rows - 2) * tileSize, 0f));      // just below top wall
		}

		// One tile inward from the left and right edges
		for (int y = 0; y < rows; y++)
		{
			spawnPositions.Add(new Vector3(1 * tileSize, y * tileSize, 0f));               // just right of left wall
			spawnPositions.Add(new Vector3((columns - 2) * tileSize, y * tileSize, 0f));   // just left of right wall
		}

		// Randomly choose a valid spawn position
		if (spawnPositions.Count == 0) return;

		Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
		GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
		Instantiate(enemy, spawnPos, Quaternion.identity);
	}

    IEnumerator SpawnEnemiesOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnDelay);
            SpawnEnemyAtEdge();
            currentSpawnDelay = Mathf.Max(currentSpawnDelay * spawnAcceleration, minSpawnDelay);
        }
    }

    public void StartSpawningEnemies()
    {
        currentSpawnDelay = initialSpawnDelay / Mathf.Max(initialEnemyCount, 1);
        spawnRoutine = StartCoroutine(SpawnEnemiesOverTime());
    }

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(enemyPrefabs, initialEnemyCount, initialEnemyCount);
        StartSpawningEnemies();
    }
}

