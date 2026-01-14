using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    private List<Resource> _availableResources = new List<Resource>();
    
    public void AddResource(Resource resource)
    {
        if (_availableResources.Contains(resource) == false)
        {
            _availableResources.Add(resource);
            SortResourcesByDistance();
        }
    }
    
    public void RemoveResource(Resource resource)
    {
        _availableResources.Remove(resource);
    }
    
    public Resource GetNearestResource()
    {
        if (_availableResources.Count == 0)
            return null;
        
        return _availableResources[0];
    }
    
    private void SortResourcesByDistance()
    {
        Vector3 castlePosition = transform.position;
        
        _availableResources = _availableResources.Where(resource => resource != null && resource.IsCollected)
            .OrderBy(resource => Vector3.Distance(castlePosition, resource.transform.position)).ToList();
    }
}
