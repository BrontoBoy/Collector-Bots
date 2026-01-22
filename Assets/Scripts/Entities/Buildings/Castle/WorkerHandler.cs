using System.Collections.Generic;
using UnityEngine;

public class WorkerHandler : MonoBehaviour
{
    [SerializeField] private List<Worker> _workers = new List<Worker>();
    [SerializeField] private int _workerCost = 5;
    [SerializeField] private int _maxWorkers = 10;
    
    private Queue<Worker> _freeWorkers = new Queue<Worker>();
    private HashSet<Worker> _freeWorkersSet = new HashSet<Worker>();
    
    public bool HasFreeWorkers => _freeWorkers.Count > 0;
    public int WorkersCount => _workers.Count;
    public int MaxWorkers => _maxWorkers;
    public int WorkerCost => _workerCost;
    
    public void Initialize()
    {
        _freeWorkers.Clear();
        _freeWorkersSet.Clear();

        foreach (var worker in _workers)
        {
            if (worker != null)
                EnqueueFreeWorker(worker);
        }
    }
    
    public Worker GetFreeWorker()
    {
        CleanQueue();

        while (_freeWorkers.Count > 0)
        {
            Worker worker = _freeWorkers.Dequeue();
            _freeWorkersSet.Remove(worker);

            if (worker != null && worker.IsFree)
                return worker;
        }

        return null;
    }
    
    public void ReturnWorkerToFree(Worker worker)
    {
        if (worker != null && worker.IsFree)
        {
            if (_freeWorkersSet.Contains(worker) == false)
            {
                _freeWorkers.Enqueue(worker);
                _freeWorkersSet.Add(worker);
            }
        }
    }
    
    public void AddWorker(Worker worker)
    {
        if (_workers.Count == _maxWorkers)
            return;

        if (worker != null && _workers.Contains(worker) == false)
        {
            _workers.Add(worker);
            ReturnWorkerToFree(worker);
        }
    }
    
    private void EnqueueFreeWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (_freeWorkersSet.Add(worker))
            _freeWorkers.Enqueue(worker);
    }
    
    private void CleanQueue()
    {
        List<Worker> validWorkers = new List<Worker>();

        while (_freeWorkers.Count > 0)
        {
            Worker worker = _freeWorkers.Dequeue();
            _freeWorkersSet.Remove(worker);

            if (worker != null && worker.IsFree)
                validWorkers.Add(worker);
        }

        foreach (Worker worker in validWorkers)
        {
            _freeWorkers.Enqueue(worker);
            _freeWorkersSet.Add(worker);
        }
    }
}