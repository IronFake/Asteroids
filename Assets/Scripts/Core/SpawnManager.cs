using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public bool spawnAliens = true;
    public bool spawnAsteroids = true;

    [SerializeField] private float alienSpawnRate = 2f;
    [SerializeField] private float asteroidsSpawnRate = 1f;
    [SerializeField] private float forceRange = 2;
    [SerializeField] private float angleRangeForSpawn = 45f;

    [Header("Prefabs")]
    public GameObject[] alienPrefabs;
    public GameObject asteroidPrefab;
    
    private Vector2 increasedSpawnBounds;
    private float spawnOffset = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        Vector2 spawnBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        increasedSpawnBounds.x = spawnBounds.x + spawnOffset;
        increasedSpawnBounds.y = spawnBounds.y + spawnOffset;
    }

    private void OnEnable()
    {
        if (spawnAsteroids == true) StartCoroutine(SpawnObject(asteroidPrefab, asteroidsSpawnRate));
        if (spawnAliens == true) StartCoroutine(SpawnRandomObjects(alienPrefabs, alienSpawnRate));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnObject(GameObject prefab, float spawnRate)
    {
        while (true)
        {
            Spawn(prefab);
            yield return new WaitForSeconds(spawnRate);
        } 
    }

    IEnumerator SpawnRandomObjects(GameObject[] prefabs, float spawnRate)
    {
        while (true)
        {
            Spawn(prefabs[Random.Range(0, prefabs.Length)]);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void Spawn(GameObject prefab)
    {
        bool spawnOnYAxis = Random.Range(0, 2) > 0;

        Vector2 spawnPosition;
        if (spawnOnYAxis)
        {
            bool isLowerSide = Random.Range(0, 2) > 0;
            spawnPosition = new Vector2(Random.Range(-increasedSpawnBounds.x, increasedSpawnBounds.x),
            isLowerSide ? -increasedSpawnBounds.y : increasedSpawnBounds.y);
        }
        else
        {
            bool isLeftSide = Random.Range(0, 2) > 0;
            spawnPosition = new Vector2(isLeftSide ? -increasedSpawnBounds.x : increasedSpawnBounds.x,
            Random.Range(-increasedSpawnBounds.y, increasedSpawnBounds.y));
        }

        //Create obstacle
        GameObject obstacle = Instantiate(prefab, spawnPosition, transform.rotation);
        Rigidbody2D rigidbody = obstacle.GetComponent<Rigidbody2D>();


        Vector2 pos = (Vector3.zero - obstacle.transform.position);
        rigidbody.rotation = Random.Range(-angleRangeForSpawn, angleRangeForSpawn);
        rigidbody.AddRelativeForce(pos.normalized * Random.Range(1, forceRange), ForceMode2D.Impulse);
    }
}
