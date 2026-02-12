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
    private bool _isSubscribedToMover = false;
    
    public event Action<Worker, Gold> GoldCollected;
    public event Action<Worker, Gold> GoldDelivered;
    public event Action<Worker, ITargetable> FlagReached;
    
    public Carrier Carrier { get; private set; }

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _animationHandler = GetComponent<AnimationHandler>();
        Carrier = GetComponent<Carrier>();
    }
    
    private void FixedUpdate()
    {
        bool isMoving = _mover != null && _mover.IsMoving;
        
        _animationHandler.SetMoving(isMoving);
    }
    
    public void SetTarget(ITargetable target)
    {
        _currentTarget = target;
        
        if (_isSubscribedToMover)
            _mover.TargetReached -= OnMoveCompleted;
            
        _mover.TargetReached += OnMoveCompleted;
        _isSubscribedToMover = true;
        
        MoveToTarget(target);
    }
    
    public void SetAsFree()
    {
        _currentTarget = null;
        _mover.StopMove();
        _animationHandler.SetMoving(false);
        
        if (_isSubscribedToMover)
        {
            _mover.TargetReached -= OnMoveCompleted;
            _isSubscribedToMover = false;
        }
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
        Collider goldCollider = gold.GetComponent<Collider>();
        
        if (goldCollider != null && goldCollider.enabled == false)
        {
            SetAsFree();
            return;
        }
        
        gold.GetComponent<Collider>().enabled = false;
        Carrier.AttachGold(gold);
        _currentTarget = null;  
        
        GoldCollected?.Invoke(this, gold);
    }
    
    private void OnMoveCompleted()
    {
        if (_isSubscribedToMover)
        {
            _mover.TargetReached -= OnMoveCompleted;
            _isSubscribedToMover = false;
        }
        
        if (_currentTarget is Gold gold)
        {
            CollectGold(gold);
        }
        else if (_currentTarget is TargetPoint && Carrier.TargetGold != null)
        {
            DepositGold();
        }
        else if (_currentTarget is Flag flag)
        {
            FlagReached?.Invoke(this, flag);
        }
    }
}