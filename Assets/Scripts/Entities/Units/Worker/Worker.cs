using System;
using UnityEngine;

[RequireComponent(typeof(Carrier))]
[RequireComponent(typeof(Collector))]
[RequireComponent(typeof(Deliverer))]
public class Worker : Unit
{
    private Carrier _carrier;
    private Collector _collector;
    private Deliverer _deliverer;
    private bool _isFree = true;
    private bool _hasResource = false;
    private ITargetable _currentTarget;
    private Resource _carriedResource;
    
    public event Action<Worker, Resource> ResourceCollected;
    public event Action<Worker, Resource> ResourceDelivered;
    public event Action<Worker> BecameFree;
    
    public bool IsFree => _isFree;
    public bool HasResource => _hasResource;

    private void Awake()
    {
        _carrier = GetComponent<Carrier>();
        _collector = GetComponent<Collector>();
        _deliverer = GetComponent<Deliverer>();
    }
    
    private void OnEnable()
    {
        if (_collector != null)
            _collector.ResourceCollected += OnResourceCollected;
        
        if (_deliverer != null)
            _deliverer.DeliveryCompleted += OnDeliveryCompleted;
    }

    private void OnDisable()
    {
        if (_collector != null)
            _collector.ResourceCollected -= OnResourceCollected;
            
        if (_deliverer != null)
            _deliverer.DeliveryCompleted -= OnDeliveryCompleted;
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
        {
            Mover.StopMove();
        }
        
        BecameFree?.Invoke(this);
    }
    
    private void OnResourceCollected(Resource resource)
    {
        if (_hasResource == false && _carrier != null)
        {
            _carrier.AttachResource(resource);
            _hasResource = true;
            
            _carriedResource = resource;
            
            ResourceCollected?.Invoke(this, resource);
        }
    }
    
    private void OnDeliveryCompleted()
    {
        if (_hasResource == false || _carrier == null)
            return;
        
        Resource deliveredResource = _carrier.DetachResource();
        _hasResource = false;
        
        ResourceDelivered?.Invoke(this, deliveredResource);
        
        _carriedResource = null;
        SetAsFree();
    }
    
    public void MoveToTarget(ITargetable target)
    {
        if (target != null)
        {
            Mover.StartMove(target.Position);
        }
    }
}