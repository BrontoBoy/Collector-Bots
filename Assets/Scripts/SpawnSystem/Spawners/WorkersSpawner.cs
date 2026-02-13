using UnityEngine;

public class WorkersSpawner : MonoBehaviour
{
    [SerializeField] private WorkerFactory _workerFactory;
    [SerializeField] private SpawnPoint _spawnPoint; 

    public SpawnPoint SpawnPoint => _spawnPoint;
    
    public Worker SpawnWorker()
    {
        if (_workerFactory == null)
            return null;

        if (_spawnPoint == null)
            return null;

        return _workerFactory.Create(_spawnPoint.transform.position);
    }
    
    public void OnWorkerSpawnRequested(Castle castle)
    {
        Worker newWorker = SpawnWorker();
        
        if (newWorker != null && castle != null)
        {
            castle.WorkerHandler.AddWorker(newWorker);
            castle.SubscribeToWorkerEvents(newWorker);
        }
    }
}