using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<SpawnPoint> SpawnPoints;
    [SerializeField] protected GameObjectsPool<T> GameObjectsPool;
    
    public List<SpawnPoint> SpawnPointsList => SpawnPoints;
    public GameObjectsPool<T> Pool => GameObjectsPool;
    
    public T SpawnRandom()
    {
        SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
        
        if (randomSpawnPoint == null)
            return null;
        
        T spawnedObject = GameObjectsPool.GetObject();
        
        if (spawnedObject == null)
            return null;
        
        spawnedObject.transform.position = randomSpawnPoint.transform.position;
        OnObjectSpawned(spawnedObject);
        
        return spawnedObject;
    }
    
    protected SpawnPoint GetRandomSpawnPoint()
    {
        if (SpawnPoints == null || SpawnPoints.Count == 0)
            return null;
            
        int randomIndex = Random.Range(0, SpawnPoints.Count);
        
        return SpawnPoints[randomIndex];
    }
    
    protected virtual void OnObjectSpawned(T spawnedObject) { }
}