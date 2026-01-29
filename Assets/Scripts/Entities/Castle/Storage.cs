using System;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private TargetPoint _deliveryPoint;
    
    private int _goldsValue = 0;

    public event Action<int> GoldsChanged;
    
    public int GoldsValue => _goldsValue;
    public TargetPoint DeliveryPoint => _deliveryPoint;
    
    public void AddGold()
    {
        _goldsValue++;
        GoldsChanged?.Invoke(_goldsValue);
    }
    
    public void SpendGold(int price)
    {
        if (_goldsValue >= price)
        {
            _goldsValue -= price;
            GoldsChanged?.Invoke(_goldsValue);
        }
    }
}