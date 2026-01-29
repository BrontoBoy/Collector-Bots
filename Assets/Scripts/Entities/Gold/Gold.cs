using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Gold : MonoBehaviour, ITargetable
{ 
    public Vector3 Position => transform.position;
    
    public void Collect()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer != null)
            renderer.enabled = false;
            
        Collider collider = GetComponent<Collider>();
            
        if (collider != null)
            collider.enabled = false;
    }
    
    public void ResetState()
    {
        transform.SetParent(null);
        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        
        if (renderer != null)
            renderer.enabled = true;
        
        Collider collider = GetComponent<Collider>();
        
        if (collider != null)
            collider.enabled = true;
    }
}