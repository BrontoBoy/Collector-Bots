using System;
using UnityEngine;

public class TargetPoint : MonoBehaviour, ITargetable
{
    public event Action<Worker> WorkerEnteredDeliveryZone;
    
    public Vector3 Position => transform.position;
    
    private void OnTriggerEnter(Collider other)
    {
        Worker worker = other.GetComponent<Worker>();
        
        if (worker != null)
        {
            if(worker.Carrier.IsCarrying)
            WorkerEnteredDeliveryZone?.Invoke(worker);
        }
    }
}