using UnityEngine;

public class Resource : Entity, IPoolable, IScannable, ITargetable
{
    private bool _isScanned = false;
    private bool _isCollected = false; 
    
    public Vector3 Position => transform.position;
    public bool IsScanned => _isScanned;
    public bool IsCollected => _isCollected;
    
    public void MarkAsScanned()
    {
        if (_isScanned == false && _isCollected == false)
        {
            _isScanned = true;
        }
    }
    
    public void Collect()
    {
        if (_isCollected == false)
        {
            _isCollected = true;
            gameObject.SetActive(false);
        }
    }
}