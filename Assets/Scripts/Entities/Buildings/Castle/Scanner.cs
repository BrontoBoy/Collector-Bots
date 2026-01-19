using System;
using System.Collections;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _delay = 2f;

    public event Action<Resource> ResourceFound;
    
    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }
    
    private IEnumerator ScanRoutine()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(_delay);
            
            FindResources();
        }
    }
    
    public void FindResources()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out Resource resource))
                {
                    if (resource.IsCollected == false)
                    {
                        resource.MarkAsScanned();
                        ResourceFound?.Invoke(resource);
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}