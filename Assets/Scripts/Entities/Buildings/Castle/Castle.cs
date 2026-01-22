using UnityEngine;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
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
    private WorkerHandler _workerHandler;
    private CastleUI _castleUI;
    private UnitsSpawner _unitsSpawner;
    private FlagHandler _flagHandler;

    // Публичные свойства для доступа извне (Game)
    public Scanner Scanner => _scanner;
    public WorkerHandler WorkerHandler => _workerHandler;

    public bool HasFlag => _flagHandler != null && _flagHandler.HasFlag;
    public Flag Flag => _flagHandler?.Flag;

    protected override void Awake()
    {
        base.Awake();

        _scanner = GetComponent<Scanner>();
        _storage = GetComponent<Storage>();
        _workerHandler = GetComponent<WorkerHandler>();
        _castleUI = GetComponent<CastleUI>();
        _unitsSpawner = GetComponent<UnitsSpawner>();
        _flagHandler = GetComponent<FlagHandler>();
    }

    private void Start()
    {
        _workerHandler.Initialize();
        _storage.ResourcesChanged += OnStorageResourcesChanged;
        UpdateCastleUI();
    }

    private void OnDisable()
    {
        _storage.ResourcesChanged -= OnStorageResourcesChanged;
    }

    private void OnStorageResourcesChanged(int newValue)
    {
        UpdateCastleUI();
        TrySpawnWorker();
    }

    // Публичный метод, который вызывает Game при глобальном назначении рабочего
    public bool TryAssignWorkerToResource(Resource resource)
    {
        if (resource == null)
            return false;

        if (_workerHandler == null || _workerHandler.HasFreeWorkers == false)
            return false;

        Worker freeWorker = _workerHandler.GetFreeWorker();
        if (freeWorker == null)
            return false;

        SubscribeToWorkerEvents(freeWorker);
        freeWorker.AssignTarget(resource);

        // Удаление ресурса из общего списка делает Game/ResourceHandler
        return true;
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

        // Возврат ресурса в пул выполняется через ResourceHandler (владелец логики)
        UpdateCastleUI();
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
    }

    private void UpdateCastleUI()
    {
        if (_castleUI != null)
            _castleUI.UpdateResourcesDisplay(_storage.ResourcesValue);
    }

    private void OnWorkerBecameFree(Worker worker)
    {
        _workerHandler.ReturnWorkerToFree(worker);
        UnsubscribeFromWorkerEvents(worker);
    }

    private void TrySpawnWorker()
    {
        if (_workerHandler == null)
            return;

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
            }
        }
    }
}
