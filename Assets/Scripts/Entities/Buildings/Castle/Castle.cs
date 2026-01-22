using UnityEngine;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(ResourceHandler))]
[RequireComponent(typeof(WorkerHandler))]
[RequireComponent(typeof(CastleUI))]
[RequireComponent(typeof(UnitsSpawner))]
[RequireComponent(typeof(BuildingRenderer))]
[RequireComponent(typeof(FlagHandler))]
[RequireComponent(typeof(MarkersSpawner))]
public class Castle : Building
{
    private Storage _storage;
    private Scanner _scanner;
    private ResourceHandler _resourceHandler;
    private WorkerHandler _workerHandler;
    private CastleUI _castleUI;
    private UnitsSpawner _unitsSpawner;
    private FlagHandler _flagHandler;
    
    public bool HasFlag => _flagHandler != null && _flagHandler.HasFlag;
    public Flag Flag => _flagHandler?.Flag;
    
    protected override void Awake()
    {
        base.Awake();
        
        _scanner = GetComponent<Scanner>();
        _storage = GetComponent<Storage>();
        _resourceHandler = GetComponent<ResourceHandler>();
        _workerHandler = GetComponent<WorkerHandler>();
        _castleUI = GetComponent<CastleUI>();
        _unitsSpawner = GetComponent<UnitsSpawner>();
        _flagHandler = GetComponent<FlagHandler>();
    }
    
    private void Start()
    {
        _workerHandler.Initialize();
        UpdateCastleUI();
    }

    private void OnEnable()
    {
        _scanner.ResourceFound += OnResourceFound;
        _resourceHandler.ResourcesListUpdated += OnResourcesListUpdated;
        _storage.ResourcesChanged += OnStorageResourcesChanged;
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= OnResourceFound;
        _resourceHandler.ResourcesListUpdated -= OnResourcesListUpdated;
        _storage.ResourcesChanged -= OnStorageResourcesChanged;
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_flagHandler != null)
            _flagHandler.PlaceFlag(position);
    }
    
    public void RemoveFlag()
    {
        if (_flagHandler != null)
            _flagHandler.RemoveFlag();
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
        worker.AssignTarget(_storage.DeliveryPoint);
    }
    
    private void OnWorkerResourceDelivered(Worker worker, Resource resource)
    {
		_storage.AddResource();
        _resourceHandler.ReturnResourceToPool(resource);
		UpdateCastleUI();
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
        TryAssignWorkerToResource();
    }
    
	private void UpdateCastleUI()
    {
		_castleUI.UpdateResourcesDisplay(_storage.ResourcesValue);
    }
    
    private void OnWorkerBecameFree(Worker worker)
    {
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
        TryAssignWorkerToResource();
    }

	private void OnStorageResourcesChanged(int newValue)
    {
        UpdateCastleUI();
        TrySpawnWorker();
    }

    private void TrySpawnWorker()
    {
        if (_workerHandler.WorkersCount >= _workerHandler.MaxWorkers)
            return;
        
        int workerCost = _workerHandler.WorkerCost;
        
        if (_storage.ResourcesValue >= workerCost)
            SpawnNewWorker(workerCost);
    }
    
    private void SpawnNewWorker(int cost)
    {
        _storage.SpendResource(cost);
    
        if (_unitsSpawner != null)
        {
            Unit newUnit = _unitsSpawner.SpawnWorker();
        
            if (newUnit is Worker newWorker)
            {
                _workerHandler.AddWorker(newWorker);
                newWorker.SetAsFree();
                TryAssignWorkerToResource();
            }
        }
	}
}