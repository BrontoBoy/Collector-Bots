using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorkersSpawner))]
public class WorkerHandler : MonoBehaviour
{
    [SerializeField] private List<Worker> _workers = new List<Worker>();
    [SerializeField] private int _workerCost = 0;
    [SerializeField] private int _maxWorkers = 10;
    [SerializeField] private WorkersSpawner _workersSpawner;
    
    private Queue<Worker> _freeWorkers = new Queue<Worker>();
    
    public int WorkersCount => _workers.Count;
    public int FreeWorkersCount => _freeWorkers.Count;
    public int MaxWorkers => _maxWorkers;
    public int WorkerCost => _workerCost;
    public IReadOnlyList<Worker> Workers => _workers.AsReadOnly();
    public WorkersSpawner WorkersSpawner => _workersSpawner;
    
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
    
    public void ReturnWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (_freeWorkers.Contains(worker))
            return;

        _freeWorkers.Enqueue(worker);
    }
    
    public void RemoveWorker(Worker worker)
    {
        if (worker == null || !_workers.Contains(worker))
            return;

        _workers.Remove(worker);
    }
}