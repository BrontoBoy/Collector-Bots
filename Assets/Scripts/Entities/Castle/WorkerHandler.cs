using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHandler : MonoBehaviour
{
    [SerializeField] private List<Worker> _workers = new List<Worker>();
    [SerializeField] private int _workerCost = 0;
    [SerializeField] private int _maxWorkers = 10;
    
    private Queue<Worker> _freeWorkers = new Queue<Worker>();
    
    public event Action<Castle> WorkerSpawnRequested;
    
    public int WorkersCount => _workers.Count;
    public int FreeWorkersCount => _freeWorkers.Count;
    public int MaxWorkers => _maxWorkers;
    public int WorkerCost => _workerCost;
    public IReadOnlyList<Worker> Workers => _workers.AsReadOnly();
    
    public void Initialize()
    {
        foreach (Worker worker in _workers)
        {
            if (worker != null)
                _freeWorkers.Enqueue(worker);
        }
    }
    
    public Worker GetFreeWorker()
    {
        if (_freeWorkers.Count > 0)
        {
            Worker worker = _freeWorkers.Dequeue();

            if (worker != null && worker.Carrier.TargetGold == null && _workers.Contains(worker))
                return worker;
            
            if (worker != null && _workers.Contains(worker))
                _freeWorkers.Enqueue(worker);
            
            if (worker != null && worker.Carrier.TargetGold == null)
                return worker;
        }

        return null;
    }
    
    public void AddWorker(Worker worker)
    {
        if (_workers.Count == _maxWorkers)
            return;

        if (worker != null && _workers.Contains(worker) == false)
        {
            _workers.Add(worker);
            _freeWorkers.Enqueue(worker);
        }
    }
    
    public void ReleaseWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (_freeWorkers.Contains(worker))
            return;

        _freeWorkers.Enqueue(worker);
    }
    
    public void RemoveWorker(Worker worker)
    {
        if (worker == null || _workers.Contains(worker) == false)
            return;

        _workers.Remove(worker);
        
        Queue<Worker> newFreeWorkers = new Queue<Worker>();
        
        foreach (Worker queuedWorker in _freeWorkers)
        {
            if (queuedWorker != worker)
                newFreeWorkers.Enqueue(queuedWorker);
        }
        
        _freeWorkers = newFreeWorkers;
    }
    
    public void RequestSpawnWorker(Castle castle)
    {
        WorkerSpawnRequested?.Invoke(castle);
    }
}