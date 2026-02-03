using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected List<SpawnPoint> SpawnPoints; // ОСТАЁТСЯ

    protected SpawnPoint GetRandomSpawnPoint()
    {
        if (SpawnPoints == null || SpawnPoints.Count == 0)
            return null;

        int randomIndex = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[randomIndex];
    }

    protected virtual T CreateInstance(T prefab) // CHANGED: без пула
    {
        return Instantiate(prefab);
    }

    protected virtual void OnObjectSpawned(T spawnedObject) { }
}