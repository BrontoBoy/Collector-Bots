using System;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public event Action<Resource> ResourceCollected;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
        {
            resource.Collect();
            ResourceCollected?.Invoke(resource);
        }
    }
}