using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour, ISpawner where T : MonoBehaviour
{
    [SerializeField] protected List<SpawnPoint> SpawnPoints;
    [SerializeField] protected float Delay = 2f;
    [SerializeField] protected GameObjectsPool<T> GameObjectsPool;

    protected Coroutine Coroutine;
    
    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }

    public void StartSpawning()
    {
        Coroutine = StartCoroutine(Spawning());
    }

    private IEnumerator Spawning()
    {
        var wait = new WaitForSeconds(Delay);

        while (enabled)
        {
            SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
            T spawnedObject = GameObjectsPool.GetObject();
            spawnedObject.transform.position = randomSpawnPoint.transform.position;

            yield return wait;
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, SpawnPoints.Count);

        return SpawnPoints[randomIndex];
    }
}