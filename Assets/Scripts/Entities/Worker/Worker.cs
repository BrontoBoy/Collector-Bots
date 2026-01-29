using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Worker : MonoBehaviour
{
    private Mover _mover; 
    private AnimationHandler _animationHandler;
    private ITargetable _currentTarget;
    
    public event Action<Worker, Gold> GoldCollected;
    public event Action<Worker, Gold> GoldDelivered;
    
    public Carrier Carrier { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _animationHandler = GetComponent<AnimationHandler>();
        Carrier = GetComponent<Carrier>();
    }
    
    private void FixedUpdate()
    {
        if (_animationHandler != null)
        {
            bool isMoving = _mover != null && _mover.IsMoving;
            _animationHandler.SetMoving(isMoving);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Gold gold))
            if (Carrier.TargetGold != null && ReferenceEquals(_currentTarget, gold))
                CollectGold(gold);
        
        if (collision.gameObject.TryGetComponent<TargetPoint>(out _))
        {
            if (Carrier.TargetGold != null)
            {
                DepositGold();
                Carrier.SetTargetGold(null);
            }
        }
    }
    
    public void SetTarget(ITargetable target)
    {
        _currentTarget = target;
        MoveToTarget(target);
    }
    
    public void SetAsFree()
    {
        _currentTarget = null;
        _mover.StopMove();
        _animationHandler.SetMoving(false);
    }
    
    public void MoveToTarget(ITargetable target)
    {
        _mover.StartMove(target.Position);
        _animationHandler.SetMoving(true);
    }
    
    public void DepositGold()
    {
        Carrier.DetachGold();
        
        GoldDelivered?.Invoke(this, Carrier.TargetGold);
    }
    
    private void CollectGold(Gold gold)
    {
        Carrier.AttachGold(gold);
            
        GoldCollected?.Invoke(this, gold);
            
        _currentTarget = null;
    }
}