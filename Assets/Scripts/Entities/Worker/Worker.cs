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
    private bool _isCarrying;
    
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
            if (_isCarrying)
                return;
            
            if (Carrier.TargetGold != null)
            {
                _isCarrying = true;
                DepositGold();
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
        _isCarrying = false;
        _currentTarget = null;
        _mover.StopMove();
        _animationHandler.SetMoving(false);
    }
    
    private void MoveToTarget(ITargetable target)
    {
        _mover.StartMove(target.Position);
        _animationHandler.SetMoving(true);
    }
    
    private void DepositGold()
    {
        Gold gold = Carrier.TargetGold;

        if (gold == null)
            return;
        
        Carrier.SetTargetGold(null);

        GoldDelivered?.Invoke(this, gold);
    }
    
    private void CollectGold(Gold gold)
    {
        Carrier.AttachGold(gold);
            
        GoldCollected?.Invoke(this, gold);
            
        _currentTarget = null;
    }
}