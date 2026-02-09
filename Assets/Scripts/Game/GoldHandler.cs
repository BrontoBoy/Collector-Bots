using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GoldsSpawner))]
public class GoldHandler : MonoBehaviour
{
    [SerializeField] private GoldsSpawner _goldsSpawner;
    
    private List<Gold> _golds = new List<Gold>();
    
    // Событие когда золото готово к сбору (без дублей)
    public event System.Action<Gold> GoldReadyForCollection;

    public GoldsSpawner GoldsSpawner => _goldsSpawner;

    private void Awake()
    {
        if (_goldsSpawner == null)
            _goldsSpawner = GetComponent<GoldsSpawner>();
    }

    public bool TryAddGold(Gold gold)
    {
        if (gold == null || _golds.Contains(gold))
            return false;

        _golds.Add(gold);
        GoldReadyForCollection?.Invoke(gold);
        return true;
    }

    public void RemoveGold(Gold gold)
    {
        if (gold == null || !_golds.Contains(gold))
            return;

        _golds.Remove(gold);
        ReturnGoldToPool(gold);
    }

    public void ReturnGoldToPool(Gold gold)
    {
        if (gold != null && _goldsSpawner != null)
            _goldsSpawner.ReturnToPool(gold);
    }

    public bool IsGoldInHandler(Gold gold) => gold != null && _golds.Contains(gold);
}