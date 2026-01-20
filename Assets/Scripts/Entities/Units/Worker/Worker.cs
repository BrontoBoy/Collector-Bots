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
        Mover.TargetReached += OnTargetReached;
    }
    
    private void OnDisable()
    {
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
        if (_carrier.IsCarrying == true)
        {
            ResourceDelivered?.Invoke(this, _carrier.CarriedResource);
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