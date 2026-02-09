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
    
    public event Action<Worker> GoldMissed;
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
                
        if (isMoving == false && _currentTarget != null && _currentTarget is Gold && Carrier.TargetGold == null)
        {
            GoldMissed?.Invoke(this);  // ← Fire event (Castle handle)
            _currentTarget = null;     // Сброс, чтобы не loop
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
        
        if (collision.gameObject.TryGetComponent(out Flag flag))
        {
            if (ReferenceEquals(_currentTarget, flag))
            {
                FlagReached?.Invoke(this, flag);
                _currentTarget = null;
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
        gold.GetComponent<Collider>().enabled = false;
        
        Carrier.AttachGold(gold);
            
        GoldCollected?.Invoke(this, gold);
            
        _currentTarget = null;
    }
}