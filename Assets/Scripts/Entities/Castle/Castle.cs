using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CastleRenderer))]
[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(WorkerHandler))]
[RequireComponent(typeof(CastleUI))]
[RequireComponent(typeof(FlagHandler))]
public class Castle : MonoBehaviour, ITargetable
{
    [SerializeField] private Scanner _scanner;
    [SerializeField] private Storage _storage;
    [SerializeField] private WorkerHandler _workerHandler;
    
    private CastleRenderer _castleRenderer;
    private CastleUI _castleUI;
    private FlagHandler _flagHandler;
    
    public event Action<Castle,Worker, Gold> GoldDelivered;
    
    public Vector3 Position => transform.position;
    public Scanner Scanner => _scanner;
    public Storage Storage => _storage;
    public WorkerHandler WorkerHandler => _workerHandler;
    public bool IsSelected { get; private set; }
    public bool HasFlag => _flagHandler != null && _flagHandler.HasFlag;
    public Flag Flag => _flagHandler?.Flag;
    
    protected void Awake()
    {   
        _castleRenderer = GetComponent<CastleRenderer>();
        _castleUI = GetComponent<CastleUI>();
        _flagHandler = GetComponent<FlagHandler>();
        _workerHandler.Initialize();
        
        foreach (Worker worker in _workerHandler.Workers)
            SubscribeToWorkerEvents(worker);
        
        UpdateCastleUI();
    }
    
    private void OnEnable()
    {
        _storage.GoldsChanged += OnStorageGoldsChanged;
    }

    private void OnDisable()
    {
        _storage.GoldsChanged -= OnStorageGoldsChanged;
        
        foreach (Worker worker in _workerHandler.Workers)
            UnsubscribeFromWorkerEvents(worker);
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
    
    public void PlaceFlag(Vector3 position)
    {
        if (_flagHandler != null)
        {
            _flagHandler.PlaceFlag(position);
        }
    }
    
    public void AssignWorkerToGold(Worker worker, Gold gold)
    {
        if (worker == null || gold == null)
            return;

        worker.Carrier.SetTargetGold(gold);
        worker.SetTarget(gold);
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
        if (_workerHandler.WorkersCount >= _workerHandler.MaxWorkers)
            return;
        
        if (_storage.GoldsValue >= _workerHandler.WorkerCost)
            SpawnNewWorker(_workerHandler.WorkerCost);
    }
    
    private void SpawnNewWorker(int cost)
    {
        _storage.SpendGold(cost);
    
        if (_workerHandler.WorkersSpawner != null)
        {
            Worker newWorker = _workerHandler.WorkersSpawner.SpawnWorker();
            _workerHandler.AddWorker(newWorker);
            SubscribeToWorkerEvents(newWorker);
        }
    }
    
    private void OnWorkerGoldDelivered(Worker worker, Gold gold)
    {
        GoldDelivered?.Invoke(this, worker, gold);
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