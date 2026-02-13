using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CastlesHandler : MonoBehaviour
{
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
    public event Action<Castle> CastleCreated;
    public event Action<Vector3, Action<Castle>> CastleSpawnRequested;
    
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
    
    private void TransferWorkerToNewCastle(Worker worker, Castle oldCastle, Castle newCastle)
    {
        if (worker == null || newCastle == null)
            return;
        
        oldCastle.WorkerHandler.RemoveWorker(worker);
        oldCastle.UnsubscribeFromWorkerEvents(worker);
        worker.SetAsFree();
        newCastle.SetWorkerToSpawnPoint(worker);
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
        CastleSpawnRequested?.Invoke(position, (newCastle) =>
        {
            TransferWorkerToNewCastle(worker, sourceCastle, newCastle);
            _castles.Add(newCastle);
            CastleCreated?.Invoke(newCastle);
        });
    }
}