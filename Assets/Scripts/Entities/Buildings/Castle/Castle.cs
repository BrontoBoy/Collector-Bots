using UnityEngine;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(ResourceHandler))]
[RequireComponent(typeof(WorkerHandler))]
[RequireComponent(typeof(CastleUI))]
[RequireComponent(typeof(UnitsSpawner))]
public class Castle : Building
{
    private Storage _storage;
    private Scanner _scanner;
    private ResourceHandler _resourceHandler;
    private WorkerHandler _workerHandler;
	private CastleUI _castleUI;
	private UnitsSpawner _unitsSpawner;

	private bool _isSpawningWorker = false;
    
    private void Awake()
    {
        _scanner = GetComponent<Scanner>();
        _storage = GetComponent<Storage>();
        _resourceHandler = GetComponent<ResourceHandler>();
        _workerHandler = GetComponent<WorkerHandler>();
		_castleUI = GetComponent<CastleUI>();
		_unitsSpawner = GetComponent<UnitsSpawner>();
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
		 Debug.Log($"Castle {name}: UI update, storage value = {_storage.ResourcesValue}");
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
        {
            SpawnNewWorker(workerCost);
        }
    }
    
    private void SpawnNewWorker(int cost)
    {
        _storage.SpendResource(cost);
    
    if (_unitsSpawner != null && _unitsSpawner.Pool != null)
    {
        Unit newUnit = _unitsSpawner.Pool.GetObject();
        
        if (newUnit is Worker newWorker)
        {
            _workerHandler.AddWorker(newWorker);
            
            if (_unitsSpawner.SpawnPointsList.Count > 0)
            {
                int randomIndex = Random.Range(0, _unitsSpawner.SpawnPointsList.Count);
                newWorker.transform.position = _unitsSpawner.SpawnPointsList[randomIndex].transform.position;
            }
            
            newWorker.SetAsFree();
            
            // ↓↓↓ ДОБАВЬ ЭТУ СТРОЧКУ:
            _isSpawningWorker = false; // Сбрасываем флаг после успешного спавна
            // ↑↑↑ ДОБАВЬ ЭТУ СТРОЧКУ ↑↑↑
            
            TryAssignWorkerToResource();
        }
        else
        {
            // ↓↓↓ ДОБАВЬ ЭТУ СТРОЧКУ (если не Worker):
            _isSpawningWorker = false; // Сбрасываем флаг если ошибка
            // ↑↑↑ ДОБАВЬ ЭТУ СТРОЧКУ ↑↑↑
        }
    }
    else
    {
        // ↓↓↓ ДОБАВЬ ЭТУ СТРОЧКУ (если спавнер null):
        _isSpawningWorker = false; // Сбрасываем флаг
        // ↑↑↑ ДОБАВЬ ЭТУ СТРОЧКУ ↑↑↑
    }
	}
}