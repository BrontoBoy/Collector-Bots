using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CastlesSpawner))]
public class CastlesHandler : MonoBehaviour
{
    [SerializeField] private CastlesSpawner _castlesSpawner;
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
    public event System.Action<Castle> CastleCreated;
    
    public IReadOnlyList<Castle> Castles => _castles.AsReadOnly();
    
    private void Awake()
    {
        foreach (Castle castle in _castles)
            SubscribeToCastle(castle);
    }

    private void OnDestroy()
    {
        foreach (Castle castle in _castles)
            UnsubscribeFromCastle(castle);
    }
    
    public Castle GetNearestCastle(Vector3 goldPosition)
    {
        if (_castles == null || _castles.Count == 0)
            return null;
    
        Castle nearestCastle;
    
        if (_castles.Count == 1)
        {
            nearestCastle = _castles[0];
            
            if (nearestCastle != null && nearestCastle.WorkerHandler != null && nearestCastle.WorkerHandler.FreeWorkersCount > 0)
                return nearestCastle;
        
            return null;
        }
        
        nearestCastle = _castles
            .Where(castle => castle != null && castle.WorkerHandler != null && castle.WorkerHandler.FreeWorkersCount > 0)
            .OrderBy(castle => (goldPosition - castle.transform.position).sqrMagnitude)
            .FirstOrDefault();
    
        return nearestCastle;
    }
    
    private Castle CreateCastleAtPosition(Vector3 position)
    {
        if (_castlesSpawner == null)
            return null;

        Castle newCastle = _castlesSpawner.SpawnCastle(position);
        
        if (newCastle != null)
        {
            _castles.Add(newCastle);
            SubscribeToCastle(newCastle);
        }
        
        return newCastle;
    }
    
    private void TransferWorkerToNewCastle(Worker worker, Castle oldCastle, Castle newCastle)
    {
        if (worker == null || newCastle == null)
            return;
        
        oldCastle.WorkerHandler.RemoveWorker(worker);
        oldCastle.UnsubscribeFromWorkerEvents(worker);
        worker.SetAsFree();
        
        if (newCastle.WorkerHandler.WorkersSpawner != null)
        {
            SpawnPoint spawnPoint = newCastle.WorkerHandler.WorkersSpawner.GetRandomSpawnPoint();
            
            if (spawnPoint != null)
                worker.transform.position = spawnPoint.transform.position;
        }
        
        newCastle.WorkerHandler.AddWorker(worker);
        newCastle.SubscribeToWorkerEvents(worker);
    }
    
    private void SubscribeToCastle(Castle castle)
    {
        if (castle == null)
            return;

        castle.CastleCreationRequested += OnCastleCreationRequested;
    }

    private void UnsubscribeFromCastle(Castle castle)
    {
        if (castle == null)
            return;

        castle.CastleCreationRequested -= OnCastleCreationRequested;
    }
    
    private void OnCastleCreationRequested(Castle sourceCastle, Worker worker, Vector3 position)
    {
        Castle newCastle = CreateCastleAtPosition(position);
        
        if (newCastle != null)
        {
            CastleCreated?.Invoke(newCastle);
            TransferWorkerToNewCastle(worker, sourceCastle, newCastle);
        }
    }
}