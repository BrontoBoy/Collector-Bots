using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CastlesSpawner))]
public class CastlesHandler : MonoBehaviour
{
    [SerializeField] private CastlesSpawner _castlesSpawner;
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
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
        // создаём новый замок
        Castle newCastle = CreateCastleAtPosition(position);

        if (newCastle != null)
        {
            // переносим работника
            TransferWorkerToNewCastle(worker, sourceCastle, newCastle);
        }
    }
    
    private void TransferWorkerToNewCastle(Worker worker, Castle oldCastle, Castle newCastle)
    {
        if (worker == null || newCastle == null)
            return;

        // Удаляем работника из старого замка
        oldCastle.WorkerHandler.Workers.Remove(worker);

        // Снимаем старые события
        oldCastle.UnsubscribeFromWorkerEvents(worker);

        // Сброс текущей цели, чтобы не было "зависших" TargetGold/TargetPoint
        worker.SetAsFree();

        // Добавляем работника в новый замок
        newCastle.WorkerHandler.AddWorker(worker);

        // Подписываем события для нового замка
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