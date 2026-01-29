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
    }
    
    private void OnEnable()
    {
        foreach (Castle castle in _castlesHandler.Castles)
        {
            if (castle != null && castle.Scanner != null)
            {
                castle.Scanner.GoldFound += OnGoldFound;
                castle.GoldDelivered += OnGoldDelivered;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Castle castle in _castlesHandler.Castles)
        {
            if (castle != null && castle.Scanner != null)
            {
                castle.Scanner.GoldFound -= OnGoldFound;
                castle.GoldDelivered -= OnGoldDelivered;
            }
        }
    }
    
    private void Start()
    {
        StartGoldSpawning();
    }
    
    private void StartGoldSpawning()
    {
        if (_goldHandler.GoldsSpawner != null)
            _goldHandler.GoldsSpawner.StartSpawning();
    }
    
    private void OnGoldFound(Gold gold)
    {
        if(_goldHandler.IsGoldInHandler(gold))
            return;
        
        _goldHandler.AddGold(gold);
        Castle nearestCastle = _castlesHandler.GetNearestCastle(gold.transform.position);
        
        if (nearestCastle == null)
            return;
        
        Worker freeWorker = nearestCastle.WorkerHandler.GetFreeWorker();

        if (freeWorker == null)
            return;
        
        freeWorker.SetTarget(gold);
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
    }
}