using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GoldsSpawner))]
public class GoldHandler : MonoBehaviour
{
    private List<Gold> _golds = new List<Gold>();
    
    public GoldsSpawner GoldsSpawner { get; private set; }

    private void Awake()
    {
        GoldsSpawner = GetComponent<GoldsSpawner>();
    }
    
    private void OnEnable()
    {
        if (GoldsSpawner != null)
            GoldsSpawner.GoldSpawned += OnGoldSpawned;
    }

    private void OnDisable()
    {
        if (GoldsSpawner != null)
            GoldsSpawner.GoldSpawned -= OnGoldSpawned;
    }
    
    public bool IsGoldInHandler(Gold gold)
    {
        return gold != null && _golds.Contains(gold);
    }
    
    public void AddGold(Gold gold)
    {
        if (gold == null)
            return;
        
        _golds.Add(gold);
    }
    
    public void RemoveGold(Gold gold)
    {
        if (gold == null)
            return;
        
        _golds.Remove(gold);
    }
    
    public void ReturnGoldToPool(Gold gold)
    {
        if (gold != null && GoldsSpawner != null)
            GoldsSpawner.ReturnGold(gold);
    }
    
    private void OnGoldSpawned(Gold gold)
    {
        AddGold(gold);
    }
}