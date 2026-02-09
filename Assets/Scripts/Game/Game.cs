using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(GoldHandler))]
[RequireComponent(typeof(CastlesHandler))]
public class Game : MonoBehaviour
{
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private CastlesHandler _castlesHandler;
    
    private InputReader _inputReader;
    
    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        
		_inputReader.GroundRightClickedWithCastle += OnGroundRightClickedWithCastle;

        foreach (Castle castle in _castlesHandler.Castles)
        {
            SubscribeToCastle(castle);
        }
        
        _castlesHandler.CastleCreated += SubscribeToCastle;
    }
    
    private void Start()
    {
        StartGoldSpawning();
    }
    
    private void OnDisable()
    {
		_inputReader.GroundRightClickedWithCastle -= OnGroundRightClickedWithCastle;

        foreach (Castle castle in _castlesHandler.Castles)
        {
            if (castle != null && castle.Scanner != null)
            {
                castle.Scanner.GoldFound -= OnGoldFound;
                castle.GoldDelivered -= OnGoldDelivered;
            }
        }
        
        _castlesHandler.CastleCreated -= SubscribeToCastle;
    }
    
    public void SubscribeToCastle(Castle castle)
    {
        if (castle == null) 
            return;
        
        if (castle.Scanner != null)
        {
            castle.Scanner.GoldFound += OnGoldFound;
            castle.GoldDelivered += OnGoldDelivered;
        }
    }
    
    private void StartGoldSpawning()
    {
        if (_goldHandler.GoldsSpawner != null)
            _goldHandler.GoldsSpawner.StartSpawning();
    }
    
    private void OnGroundRightClickedWithCastle(Castle castle, Vector3 position)
    {
        if (castle == null)
            return;

        castle.PlaceFlag(position);
    }
    
    private void OnGoldFound(Gold gold)
    {
        if(_goldHandler.IsGoldInHandler(gold))
            return;
        
        _goldHandler.AddGold(gold);
        Castle nearestCastle = _castlesHandler.GetNearestCastle(gold.transform.position);
        
        if (nearestCastle == null)
            return;

        Gold nearestGold = nearestCastle.Scanner.GetNearestGold();
        Worker freeWorker = nearestCastle.WorkerHandler.GetFreeWorker();
        
        if (freeWorker == null)
            return;

        nearestCastle.AssignWorkerToGold(freeWorker, nearestGold);
    }
    
    private void OnGoldDelivered(Castle castle, Worker worker, Gold gold)
    {
        if(castle ==null || worker == null || gold == null )
            return;
        
        _goldHandler.RemoveGold(gold);
        castle.Scanner.RemoveGold(gold);
        _goldHandler.ReturnGoldToPool(gold); 
        castle.Storage.AddGold(); 
        worker.SetAsFree(); 
        castle.WorkerHandler.ReturnWorker(worker);
    }
}