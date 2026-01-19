using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Resource : MonoBehaviour, IPoolable, ITargetable
{
	protected BoxCollider Сollider;
	protected Rigidbody Rigidbody;

    protected bool _isScanned = false;
    protected bool _isCollected = false; 
    
    public Vector3 Position => transform.position;
    public bool IsScanned => _isScanned;
    public bool IsCollected => _isCollected;
    
 	protected virtual void Awake()
    	{
        	BoxCollider collider = GetComponent<BoxCollider>();
			Rigidbody Rigidbody = GetComponent<Rigidbody>();
    	}

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
        
        if (transform.parent != null)
            transform.SetParent(null);
    }
}