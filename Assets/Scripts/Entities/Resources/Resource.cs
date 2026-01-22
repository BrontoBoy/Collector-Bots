using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour, IPoolable, ITargetable
{
    protected bool _isScanned = false;
    protected bool _isCollected = false; 
    
    public Vector3 Position => transform.position;
    public bool IsScanned => _isScanned;
    public bool IsCollected => _isCollected;
    
    public void MarkAsScanned()
    {
        if (_isScanned == false && _isCollected == false)
            _isScanned = true;
    }
    
    public void Collect()
    {
        if (_isCollected == false)
        {
            _isCollected = true;
            
            MeshRenderer renderer = GetComponent<MeshRenderer>();

            if (renderer != null)
                renderer.enabled = false;
            
            Collider collider = GetComponent<Collider>();
            
            if (collider != null)
                collider.enabled = false;
        }
    }
    
    public void ResetState()
    {
        _isScanned = false;
        _isCollected = false;
        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        
        if (renderer != null)
            renderer.enabled = true;
        
        Collider collider = GetComponent<Collider>();
        
        if (collider != null)
            collider.enabled = true;
        
        transform.SetParent(null);
    }
}