using UnityEngine;

public class Carrier : MonoBehaviour
{
    [SerializeField] private SpawnPoint _carryPoint;
    private Resource _carriedResource;
    
    public bool IsCarrying => _carriedResource != null;
    
    public void AttachResource(Resource resource)
    {
        if (resource == null || _carriedResource != null)
            return;
            
        _carriedResource = resource;
        resource.transform.SetParent(_carryPoint.transform);
        resource.transform.localPosition = Vector3.zero;
        resource.transform.localRotation = Quaternion.identity;
    }
    
    public Resource DetachResource()
    {
        if (_carriedResource == null)
            return null;
            
        Resource resource = _carriedResource;
        _carriedResource = null;
        
        resource.transform.SetParent(null);
        
        return resource;
    }
}