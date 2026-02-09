using UnityEngine;

public class WorkersSpawner : Spawner<Worker>
{
    [SerializeField] private Worker _workerPrefab;

    public Worker SpawnWorker()
    {
        SpawnPoint spawnPoint = GetRandomSpawnPoint();
        
        if (spawnPoint == null || _workerPrefab == null)
            return null;

        Worker worker = CreateInstance(_workerPrefab);
        worker.transform.position = spawnPoint.transform.position;

        OnObjectSpawned(worker);
        return worker;
    }
}