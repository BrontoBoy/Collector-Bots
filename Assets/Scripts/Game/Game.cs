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

        // Подписываемся на сканеры всех замков
        foreach (Castle castle in _castlesHandler.Castles)
        {
            SubscribeToCastleScanner(castle);
            SubscribeToCastleDelivery(castle);
        }
        
        // Подписываемся на новые замки
        _castlesHandler.CastleCreated += OnNewCastleCreated;
        
        // Подписываемся на событие готовности золота к сбору
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
        {
            castle.Scanner.GoldFound += OnScannerFoundGold;
        }
    }
    
    private void UnsubscribeFromCastleScanner(Castle castle)
    {
        if (castle?.Scanner != null)
        {
            castle.Scanner.GoldFound -= OnScannerFoundGold;
        }
    }
    
    private void SubscribeToCastleDelivery(Castle castle)
    {
        if (castle != null)
        {
            castle.GoldDelivered += OnGoldDelivered;
        }
    }
    
    private void UnsubscribeFromCastleDelivery(Castle castle)
    {
        if (castle != null)
        {
            castle.GoldDelivered -= OnGoldDelivered;
        }
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
    
    // Сканер нашел золото - передаем в хендлер для проверки
    private void OnScannerFoundGold(Gold gold)
    {
        _goldHandler.TryAddGold(gold);
    }
    
    // Золото готово к сбору (проверено на дубли)
    private void OnGoldReadyForCollection(Gold gold)
    {
        // Находим ближайший замок с доступными рабочими
        Castle nearestCastle = _castlesHandler.GetNearestCastle(gold.transform.position);
        
        if (nearestCastle == null)
            return;
        
        // Замок сам решает, какого рабочего назначить
        bool assigned = nearestCastle.AssignWorkerToGold(gold);
        
        if (!assigned)
        {
            // Если не смогли назначить рабочего (нет свободных),
            // золото остается в хендлере и может быть назначено позже
        }
    }
    
    private void OnGoldDelivered(Castle castle, Worker worker, Gold gold)
    {
        if(castle == null || worker == null || gold == null)
            return;
        
        // Удаляем золото из хендлера
        _goldHandler.RemoveGold(gold);
        
        // Обновляем состояние замка и рабочего
        castle.Storage.AddGold(); 
        worker.SetAsFree(); 
        castle.WorkerHandler.ReturnWorker(worker);
    }
}