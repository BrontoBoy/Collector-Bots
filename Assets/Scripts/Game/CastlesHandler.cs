using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(CastlesSpawner))]
public class CastlesHandler : MonoBehaviour
{
    [SerializeField] private CastlesSpawner _castlesSpawner;
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
    public event Action<Castle> CastleCreated;
    
    public IReadOnlyList<Castle> Castles => _castles.AsReadOnly();
    
    private void Awake()
    {
        foreach (Castle castle in _castles)
            SubscribeToCastle(castle);
    }
    
    public void SubscribeToCastle(Castle castle)
    {
        if (castle == null)
            return;

        castle.CastleCreationRequested += OnCastleCreationRequested;
    }

    public void UnsubscribeFromCastle(Castle castle)
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
            CastleCreated?.Invoke(newCastle);           // ← НОВОЕ
            TransferWorkerToNewCastle(worker, sourceCastle, newCastle);
        }
    }
    
    private void TransferWorkerToNewCastle(Worker worker, Castle oldCastle, Castle newCastle)
    {
        if (worker == null || newCastle == null)
            return;

        // Удаляем работника из старого замка
        oldCastle.WorkerHandler.Workers.Remove(worker);
        oldCastle.UnsubscribeFromWorkerEvents(worker);
        worker.SetAsFree();

        // ← НОВОЕ: Телепорт в random spawn point нового замка
        if (newCastle.WorkerHandler.WorkersSpawner != null)
        {
            SpawnPoint spawnPoint = newCastle.WorkerHandler.WorkersSpawner.GetRandomSpawnPoint();
            
            if (spawnPoint != null)
                worker.transform.position = spawnPoint.transform.position;
        }

        // Добавляем работника в новый замок
        newCastle.WorkerHandler.AddWorker(worker);
        newCastle.SubscribeToWorkerEvents(worker);
    }

    
    public Castle CreateCastleAtPosition(Vector3 position)
    {
        if (_castlesSpawner == null)
            return null;

        Castle newCastle = _castlesSpawner.SpawnCastle(position); // создаёт новый замок
        if (newCastle != null)
        {
            _castles.Add(newCastle); // добавляем в список
        }
        return newCastle;
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
            .OrderBy(castle => Vector3.Distance(goldPosition, castle.transform.position))
            .FirstOrDefault();
    
        return nearestCastle;
    }
    
    
}