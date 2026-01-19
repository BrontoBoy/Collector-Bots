using System;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private TargetPoint _deliveryPoint;
    
    private int _resourcesValue = 0;

    public event Action<int> ResourcesChanged;
    
    public int ResourcesValue => _resourcesValue;
    public  TargetPoint DeliveryPoint => _deliveryPoint;
    
    private void OnEnable()
    {
        _deliveryPoint.WorkerEnteredDeliveryZone += OnWorkerEnteredDeliveryZone;
    }
    
    private void OnDisable()
    {
        _deliveryPoint.WorkerEnteredDeliveryZone -= OnWorkerEnteredDeliveryZone;
    }
    
    private void OnWorkerEnteredDeliveryZone(Worker worker)
    {
        Carrier carrier = worker.GetComponent<Carrier>();
        
        if (carrier != null && carrier.IsCarrying)
        {
            Resource resource = carrier.DetachResource();
            
            if (resource == null) 
                return;
            
            AddResource();
            
            ResourcesSpawner.СommonResourcesSpawner?.ReturnResource(resource);
            worker.SetAsFree();
        }
    }
    
    public void AddResource()
    {
        _resourcesValue++;
        Debug.Log($"Storage {name}: +1 = {_resourcesValue}");
        ResourcesChanged?.Invoke(_resourcesValue);
    }
    
    public void SpendResource(int price)
    {
        _resourcesValue -= price;
        ResourcesChanged?.Invoke(_resourcesValue);
    }
}