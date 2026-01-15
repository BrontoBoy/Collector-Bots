using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    private ResourcesSpawner _resourcesSpawner;
    private List<Resource> _availableResources = new List<Resource>();
    
    public event Action ResourcesListUpdated;
    
    public bool HasAvailableResources => _availableResources.Count > 0;

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
    
    private void OnResourceSpawned(Resource resource)
    {
        AddResource(resource);
    }
    
    public void AddResource(Resource resource)
    {
        if (_availableResources.Contains(resource) == false)
        {
            _availableResources.Add(resource);
            SortResourcesByDistance();
            ResourcesListUpdated?.Invoke();
        }
    }
    
    public void RemoveResource(Resource resource)
    {
        _availableResources.Remove(resource);
        SortResourcesByDistance(); 
        ResourcesListUpdated?.Invoke();
    }
    
    public Resource GetNearestResource()
    {
        if (_availableResources.Count == 0)
            return null;
        
        return _availableResources[0];
    }
    
    public void ReturnResourceToPool(Resource resource)
    {
        if (resource != null && _resourcesSpawner != null)
        {
            _resourcesSpawner.ReturnResource(resource);
        }
    }
    
    private void SortResourcesByDistance()
    {
        Vector3 castlePosition = transform.position;
        
        _availableResources = _availableResources.Where(resource => resource != null && resource.IsCollected == false)
            .OrderBy(resource => Vector3.Distance(castlePosition, resource.transform.position)).ToList();
    }
}