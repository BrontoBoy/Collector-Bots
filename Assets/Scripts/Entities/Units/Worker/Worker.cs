using System;
using UnityEngine;

[RequireComponent(typeof(Carrier))]
public class Worker : Unit
{
    private Carrier _carrier;
    private bool _isFree = true;
    private ITargetable _currentTarget;
    
    public event Action<Worker, Resource> ResourceCollected;
    public event Action<Worker, Resource> ResourceDelivered;
    public event Action<Worker> BecameFree;
    
    public bool IsFree => _isFree;
    public Carrier Carrier => _carrier;

    protected override void Awake()
    {
        base.Awake(); 
        _carrier = GetComponent<Carrier>();
    }
    
    private void OnEnable()
    {
        if (Mover != null)
            Mover.TargetReached += OnTargetReached;
    }
    
    private void OnDisable()
    {
        if (Mover != null)
            Mover.TargetReached -= OnTargetReached;
    }
    
    private void FixedUpdate()
    {
        if (AnimationHandler != null)
        {
            bool isMoving = Mover != null && Mover.IsMoving;
            AnimationHandler.SetMoving(isMoving);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Resource resource))
        {
            if (_isFree == false && ReferenceEquals(_currentTarget, resource))
                CollectResource(resource);
        }
    }
    
    
    public void AssignTarget(ITargetable target)
    {
        if (target == null)
            return;
            
        _currentTarget = target;
        
        if (_isFree)
        {
            _isFree = false;
            
            if (_carrier != null)
                _carrier.SetCarriedResource(null);
        }
        
        MoveToTarget(target);
    }
    
    public void SetAsFree()
    {
        _isFree = true;
        _currentTarget = null;
        
        if (_carrier.IsCarrying)
            _carrier.SetCarriedResource(null);
        
        if (Mover != null)
            Mover.StopMove();
        
        if (AnimationHandler != null)
            AnimationHandler.SetMoving(false);
        
        BecameFree?.Invoke(this);
    }
    
    public void MoveToTarget(ITargetable target)
    {
        if (target != null && Mover != null)
            Mover.StartMove(target.Position);
        
        if (AnimationHandler != null)
            AnimationHandler.SetMoving(true);
    }
    
    private void OnTargetReached()
    {
        if (_carrier.IsCarrying == true)
        {
            Resource resource = _carrier.CarriedResource;
            
            ResourceDelivered?.Invoke(this, resource);
            
            _carrier.DetachResource();
            SetAsFree();
        }
    }
    
    private void CollectResource(Resource resource)
    {
        if (_carrier.IsCarrying == false && resource != null)
        {
            resource.Collect();
            _carrier.AttachResource(resource);
            
            ResourceCollected?.Invoke(this, resource);
            
            _currentTarget = null;
        }
    }
}