using System.Collections.Generic;
using UnityEngine;

public class WorkerHandler : MonoBehaviour
{
    [SerializeField] private List<Worker> _workers = new List<Worker>();
    [SerializeField] private int _workerCost = 5;
    [SerializeField] private int _maxWorkers = 10;
    
    private Queue<Worker> _freeWorkers = new Queue<Worker>();
    
    public bool HasFreeWorkers => _freeWorkers.Count > 0;
    public int WorkersCount => _workers.Count;
    public int MaxWorkers => _maxWorkers;
    public int WorkerCost => _workerCost;
    public bool CanAddMoreWorkers => WorkersCount < MaxWorkers;
    
    public void Initialize()
    {
        _freeWorkers.Clear();
        
        foreach (var worker in _workers)
        {
            if (worker != null)
            {
                _freeWorkers.Enqueue(worker);
            }
        }
    }
    
    public Worker GetFreeWorker()
    {
        CleanQueue();
        
        if (_freeWorkers.Count == 0)
            return null;
            
        Worker worker = _freeWorkers.Dequeue();
        
        if (worker != null && worker.IsFree)
            return worker;
            
        return null;
    }
    
    public void ReturnWorkerToFree(Worker worker)
    {
        if (worker != null && worker.IsFree)
        {
            bool alreadyInQueue = false;
            
            foreach (var w in _freeWorkers)
            {
                if (w == worker)
                {
                    alreadyInQueue = true;
                    break;
                }
            }
            
            if (alreadyInQueue == false)
                _freeWorkers.Enqueue(worker);
        }
    }
    
    public void AddWorker(Worker worker)
    {
        if (worker != null && _workers.Contains(worker) == false && CanAddMoreWorkers)
            // ↑↑↑ ИЗМЕНИЛИ: добавили проверку CanAddMoreWorkers ↑↑↑
        {
            _workers.Add(worker);
            ReturnWorkerToFree(worker);
        }
    }
    
    private void CleanQueue()
    {
        List<Worker> validWorkers = new List<Worker>();
        
        while (_freeWorkers.Count > 0)
        {
            Worker worker = _freeWorkers.Dequeue();
            if (worker != null && worker.IsFree)
            {
                validWorkers.Add(worker);
            }
        }
        
        foreach (Worker worker in validWorkers)
        {
            _freeWorkers.Enqueue(worker);
        }
    }
}