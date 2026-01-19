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
        if (_isSpawningWorker) return;
        
        // Используем значения из WorkerHandler
        if (_workerHandler.WorkersCount >= _workerHandler.MaxWorkers)
        {
            return;
        }
        
        // Берем стоимость из WorkerHandler
        if (_storage.ResourcesValue >= _workerHandler.WorkerCost)
        {
            _isSpawningWorker = true;
            SpawnNewWorker(_workerHandler.WorkerCost);
        }
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
        if (_isSpawningWorker) return;
        // ↑↑↑ ДОБАВИЛИ: проверка на повторный спавн ↑↑↑

        // ↓↓↓ ИЗМЕНИЛИ: используем значения из WorkerHandler ↓↓↓
        if (_workerHandler.WorkersCount >= _workerHandler.MaxWorkers)
            return;
        
        int workerCost = _workerHandler.WorkerCost;
        // ↑↑↑ ИЗМЕНИЛИ: используем значения из WorkerHandler ↑↑↑
        
        if (_storage.ResourcesValue >= workerCost)
        {
            // ↓↓↓ ДОБАВИЛИ ↓↓↓
            _isSpawningWorker = true;
            // ↑↑↑ ДОБАВИЛИ ↑↑↑
            SpawnNewWorker(workerCost);
        }
    }
    
    private void SpawnNewWorker(int cost)
    {
        _storage.SpendResource(cost);

        if (_unitsSpawner != null && _unitsSpawner.Pool != null)
        {
            Unit newUnit = _unitsSpawner.Pool.GetObject();
        
            if (newUnit == null)
            {
                Debug.LogError("Pool returned null!");
                _isSpawningWorker = false; // ← ВАЖНО: сбрасываем флаг
                return;
            }
        
            if (newUnit is Worker newWorker)
            {
                _workerHandler.AddWorker(newWorker);
            
                if (_unitsSpawner.SpawnPointsList.Count > 0)
                {
                    int randomIndex = Random.Range(0, _unitsSpawner.SpawnPointsList.Count);
                    newWorker.transform.position = _unitsSpawner.SpawnPointsList[randomIndex].transform.position;
                }
            
                newWorker.SetAsFree();
                _isSpawningWorker = false;
                TryAssignWorkerToResource();
            }
            else
            {
                Debug.LogError("Spawned unit is not a Worker!");
                _isSpawningWorker = false;
            }
        }
        else
        {
            Debug.LogError("UnitsSpawner or Pool is null!");
            _isSpawningWorker = false;
        }
    }
}