using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<SpawnPoint> SpawnPoints;

    public SpawnPoint GetRandomSpawnPoint()
    {
        if (SpawnPoints == null || SpawnPoints.Count == 0)
            return null;

        int randomIndex = Random.Range(0, SpawnPoints.Count);
        
        return SpawnPoints[randomIndex];
    }

    protected virtual T CreateInstance(T prefab)
    {
        return Instantiate(prefab);
    }

    protected virtual void OnObjectSpawned(T spawnedObject) { }
}