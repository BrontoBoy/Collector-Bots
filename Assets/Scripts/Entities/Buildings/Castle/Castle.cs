using UnityEngine;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(ResourceHandler))]
[RequireComponent(typeof(WorkerHandler))]
public class Castle : Building
{
    private Storage _storage;
    private Scanner _scanner;
    private ResourceHandler _resourceHandler;
    private WorkerHandler _workerHandler;
    
    private void Awake()
    {
        _scanner = GetComponent<Scanner>();
        _storage = GetComponent<Storage>();
        _resourceHandler = GetComponent<ResourceHandler>();
        _workerHandler = GetComponent<WorkerHandler>();
    }
    
    private void Start()
    {
        _workerHandler.Initialize();
    }

    private void OnEnable()
    {
        _scanner.ResourceFound += OnResourceFound;
        
        if (_resourceHandler != null)
            _resourceHandler.ResourcesListUpdated += OnResourcesListUpdated;
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= OnResourceFound;
        
        if (_resourceHandler != null)
            _resourceHandler.ResourcesListUpdated -= OnResourcesListUpdated;
    }

    private void OnResourceFound(Resource resource)
    {
        _resourceHandler.AddResource(resource);
        TryAssignWorkerToResource();
    }
    
    private void OnResourcesListUpdated()
    {
        TryAssignWorkerToResource();
    }
    
    private void TryAssignWorkerToResource()
    {
        if (_workerHandler.HasFreeWorkers == false)
            return;
        
        Resource nearestResource = _resourceHandler.GetNearestResource();
        
        if (nearestResource == null)
            return;
        
        Worker freeWorker = _workerHandler.GetFreeWorker();
        
        if (freeWorker != null)
            AssignWorkerToResource(freeWorker, nearestResource);
    }
    
    private void AssignWorkerToResource(Worker worker, Resource resource)
    {
        if (worker == null || resource == null || worker.IsFree == false)
            return;
        
        SubscribeToWorkerEvents(worker);
        worker.AssignTarget(resource);
        _resourceHandler.RemoveResource(resource);
    }
    
    private void SubscribeToWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
        
        worker.ResourceCollected += OnWorkerResourceCollected;
        worker.ResourceDelivered += OnWorkerResourceDelivered;
        worker.BecameFree += OnWorkerBecameFree;
    }
    
    private void UnsubscribeFromWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
            
        worker.ResourceCollected -= OnWorkerResourceCollected;
        worker.ResourceDelivered -= OnWorkerResourceDelivered;
        worker.BecameFree -= OnWorkerBecameFree;
    }
    
    private void OnWorkerResourceCollected(Worker worker, Resource resource)
    {
        worker.AssignTarget(this);
    }
    
    private void OnWorkerResourceDelivered(Worker worker, Resource resource)
    {
        _storage.AddResource();
        _resourceHandler.ReturnResourceToPool(resource);
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
        TryAssignWorkerToResource();
    }
    
    private void OnWorkerBecameFree(Worker worker)
    {
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
        TryAssignWorkerToResource();
    }
}