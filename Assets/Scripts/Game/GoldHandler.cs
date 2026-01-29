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
    
    public void AddGold(Gold gold)
    {
        if (_golds.Contains(gold) == false)
            _golds.Add(gold);
    }
    
    public void RemoveGold(Gold gold)
    {
        _golds.Remove(gold);
    }
    
    public void ReturnGoldToPool(Gold gold)
    {
        if (gold != null && GoldsSpawner != null)
        {
            gold.ResetState();
            GoldsSpawner.ReturnGold(gold);
        }
    }
    
    private void OnGoldSpawned(Gold gold)
    {
        AddGold(gold);
    }
}