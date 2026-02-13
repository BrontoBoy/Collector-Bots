using UnityEngine;

[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(GoldHandler))]
[RequireComponent(typeof(CastlesHandler))]
public class Game : MonoBehaviour
{
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private CastlesHandler _castlesHandler;
    [SerializeField] private GoldsSpawner _goldsSpawner;
    [SerializeField] private CastlesSpawner _castlesSpawner;
   
    private void Awake()
    {
        foreach (Castle castle in _castlesHandler.Castles)
        {
            SubscribeToCastleScanner(castle);
            SubscribeToCastleDelivery(castle);
        }
       
        _castlesHandler.CastleCreated += OnNewCastleCreated;
        _goldHandler.GoldReadyForCollection += OnGoldReadyForCollection;
        
        if (_goldsSpawner != null)
            _goldHandler.GoldCollected += _goldsSpawner.OnGoldCollected;

        if (_castlesSpawner != null)
            _castlesHandler.CastleSpawnRequested += _castlesSpawner.OnCastleSpawnRequested;
    }
   
    private void OnDisable()
    {
        foreach (Castle castle in _castlesHandler.Castles)
        {
            UnsubscribeFromCastleScanner(castle);
            UnsubscribeFromCastleDelivery(castle);
        }
       
        _castlesHandler.CastleCreated -= OnNewCastleCreated;
        _goldHandler.GoldReadyForCollection -= OnGoldReadyForCollection;
        
        if (_goldsSpawner != null)
            _goldHandler.GoldCollected -= _goldsSpawner.OnGoldCollected;

        if (_castlesSpawner != null)
            _castlesHandler.CastleSpawnRequested -= _castlesSpawner.OnCastleSpawnRequested;
    }
    
    private void SubscribeToCastleScanner(Castle castle)
    {
        if (castle?.Scanner != null)
            castle.Scanner.GoldFound += OnScannerFoundGold;
    }
   
    private void UnsubscribeFromCastleScanner(Castle castle)
    {
        if (castle?.Scanner != null)
            castle.Scanner.GoldFound -= OnScannerFoundGold;
    }
   
    private void SubscribeToCastleDelivery(Castle castle)
    {
        if (castle != null)
            castle.GoldDelivered += OnGoldDelivered;
    }
   
    private void UnsubscribeFromCastleDelivery(Castle castle)
    {
        if (castle != null)
            castle.GoldDelivered -= OnGoldDelivered;
    }
   
    private void OnNewCastleCreated(Castle newCastle)
    {
        SubscribeToCastleScanner(newCastle);
        SubscribeToCastleDelivery(newCastle);
    }
   
    private void OnScannerFoundGold(Gold gold)
    {
        _goldHandler.TryAddGold(gold);
    }
   
    private void OnGoldReadyForCollection(Gold gold)
    {
        Castle nearestCastle = _castlesHandler.GetNearestCastle(gold.transform.position);
       
        if (nearestCastle == null)
            return;
       
        bool assigned = nearestCastle.AssignWorkerToGold(gold);
        
        if (assigned)
            _goldHandler.MarkAsAssigned(gold);
    }
   
    private void OnGoldDelivered(Castle castle, Worker worker, Gold gold)
    {
        if(castle == null || worker == null || gold == null)
            return;
       
        _goldHandler.RemoveGold(gold);
        worker.SetAsFree();
        castle.WorkerHandler.ReleaseWorker(worker);
        castle.Storage.AddGold();
        ProcessPendingGold();
    }
    
    private void ProcessPendingGold()
    {
        Gold freeGold = _goldHandler.GetFreeGold();
        
        if (freeGold == null)
            return;
            
        Castle nearestCastle = _castlesHandler.GetNearestCastle(freeGold.transform.position);
        
        if (nearestCastle == null)
            return;
            
        bool assigned = nearestCastle.AssignWorkerToGold(freeGold);
        
        if (assigned)
            _goldHandler.MarkAsAssigned(freeGold);
    }
}