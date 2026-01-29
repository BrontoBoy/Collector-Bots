using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class Game : MonoBehaviour
{
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
    private InputReader _inputReader;
    
    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
    }
    
    private void OnEnable()
    {
        foreach (Castle castle in _castles)
        {
            if (castle != null && castle.Scanner != null)
            {
                castle.Scanner.GoldFound += OnGoldFound;
            
                if (castle.WorkerHandler != null)
                    foreach (Worker worker in castle.WorkerHandler.Workers)
                        if (worker != null)
                            worker.GoldDelivered += OnGoldDelivered;
            }
        }
    }

    private void OnDisable()
    {
        foreach (Castle castle in _castles)
        {
            if (castle != null && castle.Scanner != null)
            {
                castle.Scanner.GoldFound -= OnGoldFound;
            
                if (castle.WorkerHandler != null)
                    foreach (Worker worker in castle.WorkerHandler.Workers)
                        if (worker != null)
                            worker.GoldDelivered -= OnGoldDelivered;
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
    
    private Castle GetNearestCastle()
    {
        List<Castle> sortedCastles = new List<Castle>();
        
        sortedCastles = _castles.Where(castle => castle != null)
            .OrderBy(castle => Vector3.Distance(castle.transform.position, castle.transform.position)).ToList();
        
        return sortedCastles[0];
    }
    
    private void OnGoldFound(Gold gold)
    {
        Castle nearestCastle = GetNearestCastle();
        
        Worker freeWorker = nearestCastle.WorkerHandler.GetFreeWorker();
        
        if(freeWorker == null)
            return;
        
        nearestCastle.Scanner.AddGold(gold);
        _goldHandler.RemoveGold(gold);
        freeWorker.SetTarget(gold);
        nearestCastle.Scanner.RemoveGold(gold);
    }

    private void OnGoldDelivered(Worker worker, Gold gold)
    {
        _goldHandler.ReturnGoldToPool(gold);
    }
}