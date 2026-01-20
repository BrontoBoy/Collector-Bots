using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    private ResourcesSpawner _resourcesSpawner;
    private List<Resource> _foundResources = new List<Resource>();
    
    public event Action ResourcesListUpdated;

    private void Awake()
    {
        _resourcesSpawner = ResourcesSpawner.СommonResourcesSpawner;
    }
    
    private void OnEnable()
    {
        if (_resourcesSpawner != null)
        {
            _resourcesSpawner.ResourceSpawned += OnResourceSpawned;
        }
    }

    private void OnDisable()
    {
        if (_resourcesSpawner != null)
        {
            _resourcesSpawner.ResourceSpawned -= OnResourceSpawned;
        }
    }
    
    public void AddResource(Resource resource)
    {
        if (_foundResources.Contains(resource) == false)
        {
            _foundResources.Add(resource);
            SortResourcesByDistance();
            ResourcesListUpdated?.Invoke();
        }
    }
    
    public void RemoveResource(Resource resource)
    {
        _foundResources.Remove(resource);
        SortResourcesByDistance(); 
        ResourcesListUpdated?.Invoke();
    }
    
    public Resource GetNearestResource()
    {
        if (_foundResources.Count == 0)
            return null;
        
        return _foundResources[0];
    }
    
    public void ReturnResourceToPool(Resource resource)
    {
        if (resource != null && _resourcesSpawner != null)
        {
            RemoveResource(resource);
            resource.ResetState();
            _resourcesSpawner.ReturnResource(resource);
        }
    }
    
    private void SortResourcesByDistance()
    {
        Vector3 castlePosition = transform.position;
        
        _foundResources = _foundResources.Where(resource => resource != null && resource.IsCollected == false)
            .OrderBy(resource => Vector3.Distance(castlePosition, resource.transform.position)).ToList();
    }
    
    private void OnResourceSpawned(Resource resource)
    {
        AddResource(resource);
    }
}