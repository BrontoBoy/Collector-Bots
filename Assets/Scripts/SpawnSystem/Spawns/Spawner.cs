using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<SpawnPoint> SpawnPoints;
    [SerializeField] protected float Delay = 2f;
    [SerializeField] protected GameObjectsPool<T> GameObjectsPool;
    
    protected Coroutine Coroutine;
    protected bool AutoStart = true;
    
    public List<SpawnPoint> SpawnPointsList => SpawnPoints;
    public GameObjectsPool<T> Pool => GameObjectsPool;
    
    protected void OnEnable()
    {
        if (AutoStart)
        {
            StartSpawning();
        }
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
        if (Coroutine == null)
            Coroutine = StartCoroutine(Spawning());
    }

    private IEnumerator Spawning()
    {
        var wait = new WaitForSeconds(Delay);

        while (enabled)
        {
            SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
            
            if (randomSpawnPoint == null)
            {
                yield return wait;
                continue;
            }
            
            T spawnedObject = GameObjectsPool.GetObject();
            
            if (spawnedObject == null)
            {
                yield return wait;
                continue;
            }
            
            spawnedObject.transform.position = randomSpawnPoint.transform.position;
            OnObjectSpawned(spawnedObject);

            yield return wait;
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        if (SpawnPoints == null || SpawnPoints.Count == 0)
            return null;
            
        int randomIndex = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[randomIndex];
    }
    
    protected virtual void OnObjectSpawned(T spawnedObject) { }
}