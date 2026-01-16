using System;
using UnityEngine;

[RequireComponent(typeof(Carrier))]
public class Worker : Unit
{
    private Carrier _carrier;
    private bool _isFree = true;
    private bool _hasResource = false;
    private ITargetable _currentTarget;
    private Resource _carriedResource;
    
    public event Action<Worker, Resource> ResourceCollected;
    public event Action<Worker, Resource> ResourceDelivered;
    public event Action<Worker> BecameFree;
    
    public bool IsFree => _isFree;
    public bool HasResource => _hasResource;

    protected override void Awake()
    {
        base.Awake(); 
        _carrier = GetComponent<Carrier>();
        
        if (Mover != null)
            Mover.TargetReached += OnTargetReached;
    }
    
    private void Start()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        
        if (collider != null)
            collider.isTrigger = false;
    }
    
    private void OnDisable()
    {
        if (Mover != null)
            Mover.TargetReached -= OnTargetReached;
    }
    
    private void FixedUpdate()
    {
        if (Animator != null)
        {
            bool isMoving = Mover != null && Mover.IsMoving;
        
            if (isMoving == true)
            {
                Animator.SetFloat("Speed", 1f);
            }
            else
            {
                Animator.SetFloat("Speed", 0f);
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Resource resource))
        {
            if (_isFree == false && _currentTarget == resource)
                CollectResource(resource);
        }
    }
    
    private void CollectResource(Resource resource)
    {
        if (_hasResource == false && _carrier != null && resource != null)
        {
            resource.Collect();
            _carrier.AttachResource(resource);
            _hasResource = true;
            _carriedResource = resource;
            
            ResourceCollected?.Invoke(this, resource);
        }
    }
    
    public void AssignTarget(ITargetable target)
    {
        if (_isFree == false || target == null)
            return;
            
        _currentTarget = target;
        _isFree = false;
        _hasResource = false;
        _carriedResource = null;
        MoveToTarget(target);
    }
    
    public void SetAsFree()
    {
        _isFree = true;
        _hasResource = false;
        _currentTarget = null;
        _carriedResource = null;
        
        if (Mover != null)
            Mover.StopMove();
        
        if (Animator != null)
            Animator.SetFloat("Speed", 0f);
        
        BecameFree?.Invoke(this);
    }
    
    public void MoveToTarget(ITargetable target)
    {
        if (target != null && Mover != null)
            Mover.StartMove(target.Position);
        
        if (Animator != null)
            Animator.SetFloat("Speed", 1f);
    }
    
    private void OnTargetReached()
    {
        if (_hasResource == true && _carriedResource != null)
        {
            ResourceDelivered?.Invoke(this, _carriedResource);
        }
    }
}