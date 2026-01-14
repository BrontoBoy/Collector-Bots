using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _delay = 2f;

    public event Action<Resource> ResourceFound;
    
    public float Delay => _delay;

    public void FindResources()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out Resource resource))
                {
                    ResourceFound?.Invoke(resource);
                }
            }
        }
    }
}
