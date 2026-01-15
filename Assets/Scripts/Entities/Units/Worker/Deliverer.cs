using System;
using UnityEngine;

public class Deliverer : MonoBehaviour
{
    public event Action DeliveryCompleted;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Storage storage))
        {
            DeliveryCompleted?.Invoke();
        }
    }
}