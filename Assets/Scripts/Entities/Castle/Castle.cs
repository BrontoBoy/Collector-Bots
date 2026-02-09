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
    [SerializeField] private int _castleCost = 5;

    private CastleRenderer _castleRenderer;
    private CastleUI _castleUI;
    private FlagHandler _flagHandler;
    private State _state = State.Normal;
    private Worker _builder;
    private bool _isBuilderMoveToCreateNewCastle = false;
    
    public event Action<Castle,Worker, Gold> GoldDelivered;
    public event Action<Castle, Worker, Vector3> CastleCreationRequested;
    
    public Vector3 Position => transform.position;
    public Scanner Scanner => _scanner;
    public Storage Storage => _storage;
    public WorkerHandler WorkerHandler => _workerHandler;
    public FlagHandler FlagHandler => _flagHandler;
    public bool IsSelected { get; private set; }
    public bool HasFlag => _flagHandler != null && _flagHandler.HasFlag;
    public Flag Flag => _flagHandler?.Flag;
    
    
    protected void Awake()
    {
        if (_scanner == null)
            _scanner  = GetComponent<Scanner>();
        
        if (_storage == null)
            _storage  = GetComponent<Storage>();
        
        if (_workerHandler == null)
            _workerHandler  = GetComponent<WorkerHandler>();
        
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
            _state = State.StartBuilding;

            if (_isBuilderMoveToCreateNewCastle == true)
                _builder.SetTarget(_flagHandler.Flag);
        }
    }
    
    public void AssignWorkerToGold(Worker worker, Gold gold)
    {
        if (worker == null || gold == null)
            return;

        worker.Carrier.SetTargetGold(gold);
        worker.SetTarget(gold);
    }

	public void SubscribeToWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
        
        worker.GoldCollected += OnWorkerGoldCollected;
        worker.GoldDelivered += OnWorkerGoldDelivered;
        worker.FlagReached += OnWorkerReachedFlag;
    }
    
    public void UnsubscribeFromWorkerEvents(Worker worker)
    {
        if (worker == null)
            return;
            
        worker.GoldCollected -= OnWorkerGoldCollected;
        worker.GoldDelivered -= OnWorkerGoldDelivered;
        worker.FlagReached -= OnWorkerReachedFlag;
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
    
    private void TryCreateCastle()
    {
        if (_storage.GoldsValue < _castleCost)
            return;

        Worker freeWorker = _workerHandler.GetFreeWorker();
        
        if (freeWorker == null)
            return;

        _storage.SpendGold(_castleCost);

        _isBuilderMoveToCreateNewCastle = true;
        _builder = freeWorker;
        _builder.FlagReached += OnWorkerReachedFlag;

        _builder.SetTarget(_flagHandler.Flag);
    }

	private void UpdateCastleUI()
    {
        _castleUI.UpdateGoldsDisplay(_storage.GoldsValue);
    }

    private void OnWorkerGoldCollected(Worker worker, Gold resource)
    {
        worker.SetTarget(_storage.DeliveryPoint);
    }
    
    private void OnWorkerReachedFlag(Worker worker, ITargetable target)
    {
        if (_flagHandler.HasFlag && target.Equals(_flagHandler.Flag))
        {
            _builder.FlagReached -= OnWorkerReachedFlag;
            Vector3 flagPosition = _flagHandler.Flag.Position;
            
            CastleCreationRequested?.Invoke(this, worker, flagPosition);

            _flagHandler.RemoveFlag();
            worker.SetAsFree();
            _workerHandler.ReturnWorker(worker);
            _isBuilderMoveToCreateNewCastle = false;
            _builder = null;
            _state = State.Normal;
        }
    }
    
    private void OnStorageGoldsChanged(int newValue)
    {
        UpdateCastleUI();
        
        switch (_state)
        {
            case State.Normal:
                TrySpawnWorker();
                break;

            case State.StartBuilding:
                TryCreateCastle();
                break;
        }
    }

    private void OnWorkerGoldDelivered(Worker worker, Gold gold)
    {
        GoldDelivered?.Invoke(this, worker, gold);
    }
    
    private enum State
    {
        Normal,
        StartBuilding
    }
}