using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(BuildingRenderer))]

public class Building : MonoBehaviour, IPoolable, ITargetable
{
    protected BuildingRenderer BuildingRenderer;
    
    public Vector3 Position => transform.position;
    public bool IsSelected { get; protected set; }
    
    protected virtual void Awake()
    {
        BuildingRenderer = GetComponent<BuildingRenderer>();
    }
    
    public virtual void Select()
    {
        IsSelected = true;
        
        if (BuildingRenderer != null)
            BuildingRenderer.OnClick();
    }
    
    public virtual void Deselect()
    {
        IsSelected = false;
        
        if (BuildingRenderer != null)
            BuildingRenderer.OnDefault();
    }
}