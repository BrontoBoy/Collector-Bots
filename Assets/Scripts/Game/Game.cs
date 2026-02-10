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
            SubscribeToCastleScanner(castle);
            SubscribeToCastleDelivery(castle);
        }
        
        _castlesHandler.CastleCreated += OnNewCastleCreated;
        _goldHandler.GoldReadyForCollection += OnGoldReadyForCollection;
    }
    
    private void OnDisable()
    {
        _inputReader.GroundRightClickedWithCastle -= OnGroundRightClickedWithCastle;

        foreach (Castle castle in _castlesHandler.Castles)
        {
            UnsubscribeFromCastleScanner(castle);
            UnsubscribeFromCastleDelivery(castle);
        }
        
        _castlesHandler.CastleCreated -= OnNewCastleCreated;
        _goldHandler.GoldReadyForCollection -= OnGoldReadyForCollection;
    }
    
    private void Start()
    {
        StartGoldSpawning();
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
    }
    
    private void OnGoldDelivered(Castle castle, Worker worker, Gold gold)
    {
        if(castle == null || worker == null || gold == null)
            return;
        
        _goldHandler.RemoveGold(gold);
        castle.Storage.AddGold(); 
        worker.SetAsFree(); 
        castle.WorkerHandler.ReturnWorker(worker);
    }
}