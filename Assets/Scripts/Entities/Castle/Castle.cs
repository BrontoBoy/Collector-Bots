using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CastleRenderer))]
[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(WorkerHandler))]
[RequireComponent(typeof(CastleUI))]
[RequireComponent(typeof(CastleRenderer))]
[RequireComponent(typeof(FlagHandler))]
public class Castle : MonoBehaviour, ITargetable
{
    private CastleRenderer _castleRenderer;
    private Storage _storage;
    private CastleUI _castleUI;
    private FlagHandler _flagHandler;
    
    public Vector3 Position => transform.position;
    public Scanner Scanner { get; private set; }
    public WorkerHandler WorkerHandler { get; private set; }
    public bool IsSelected { get; private set; }
    public bool HasFlag => _flagHandler != null && _flagHandler.HasFlag;
    public Flag Flag => _flagHandler?.Flag;
    
    protected void Awake()
    {
        _castleRenderer = GetComponent<CastleRenderer>();
        _storage = GetComponent<Storage>();
        _castleUI = GetComponent<CastleUI>();
        _flagHandler = GetComponent<FlagHandler>();
        
        Scanner = GetComponent<Scanner>();
        WorkerHandler = GetComponent<WorkerHandler>();
        
        Scanner.StartScanning();
        WorkerHandler.Initialize();
        UpdateCastleUI();
    }
    
    private void OnEnable()
    {
        Scanner.GoldFound += OnGoldFound;
        _storage.GoldsChanged += OnStorageGoldsChanged;
    }

    private void OnDisable()
    {
        Scanner.GoldFound -= OnGoldFound;
        _storage.GoldsChanged -= OnStorageGoldsChanged;
    }
    
    public void Select()
    {
        IsSelected = true;
        
        if (_castleRenderer != null)
            _castleRenderer.OnClick();
    }
    
    public void Deselect()
    {
        IsSelected = false;
        
        if (_castleRenderer != null)
            _castleRenderer.OnDefault();
    }
    
    private void OnWorkerGoldCollected(Worker worker, Gold resource)
    {
        worker.SetTarget(_storage.DeliveryPoint);
    }
    
    private void UpdateCastleUI()
    {
        _castleUI.UpdateGoldsDisplay(_storage.GoldsValue);
    }
    
    private void OnStorageGoldsChanged(int newValue)
    {
        UpdateCastleUI();
        TrySpawnWorker();
    }

    private void TrySpawnWorker()
    {
        if (WorkerHandler.WorkersCount >= WorkerHandler.MaxWorkers)
            return;
        
        if (_storage.GoldsValue >= WorkerHandler.WorkerCost)
            SpawnNewWorker(WorkerHandler.WorkerCost);
    }
    
    private void SpawnNewWorker(int cost)
    {
        _storage.SpendGold(cost);
    
        if (WorkerHandler.WorkersSpawner != null)
        {
            Worker newWorker = WorkerHandler.WorkersSpawner.SpawnWorker();
            WorkerHandler.AddWorker(newWorker);
            SubscribeToWorkerEvents(newWorker);
            newWorker.SetAsFree();
            AssignWorkerToGold();
        }
    }
    
    private void OnGoldFound(Gold gold)
    {
        AssignWorkerToGold();
    } 
    
    private void AssignWorkerToGold()
    {
        while (WorkerHandler.FreeWorkersCount > 0 && Scanner.FoundGoldsCount > 0)
        {
            Gold gold = Scanner.GetNearestGold();
            Worker worker = WorkerHandler.GetFreeWorker();
        
            if (worker != null && gold != null)
            {
                worker.Carrier.SetTargetGold(gold);
                SubscribeToWorkerEvents(worker);
                worker.SetTarget(gold);
            }
        }
    }
    
    private void OnWorkerGoldDelivered(Worker worker, Gold gold)
    {
        if (gold == null) 
            return;
        
        _storage.AddGold();
        UpdateCastleUI();
        worker.SetAsFree();
        UnsubscribeFromWorkerEvents(worker);
        AssignWorkerToGold();
    }
    
    private void SubscribeToWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
        
        worker.GoldCollected += OnWorkerGoldCollected;
        worker.GoldDelivered += OnWorkerGoldDelivered;
    }
    
    private void UnsubscribeFromWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
            
        worker.GoldCollected -= OnWorkerGoldCollected;
        worker.GoldDelivered -= OnWorkerGoldDelivered;
    }
}