using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private float _delay;

    public event Action<GoldenOre> GoldenOreFound;
    
    public float Delay => _delay;

    public void FindResources()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out GoldenOre goldenOre))
                {
                    GoldenOreFound?.Invoke(goldenOre);
                }
            }
        }
    }
}
