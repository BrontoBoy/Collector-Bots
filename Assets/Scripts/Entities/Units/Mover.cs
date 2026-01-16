using System;
using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)] private float _speed = 5f;
    [SerializeField] private float _stopDistance = 0.5f;

    private Coroutine _moveCoroutine;
    private bool _isMoving = false;
    
    public event Action TargetReached;
    
    public bool IsMoving => _isMoving;
    public float CurrentSpeed => _isMoving ? _speed : 0f;

    public void StartMove(Vector3 target)
    {
        StopMove();
        
        _moveCoroutine = StartCoroutine(MoveToPosition(target));
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        _isMoving = true;
        
        while (Vector3.Distance(transform.position, target) > _stopDistance)
        {
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            yield return null;
        }
        
        _isMoving = false;
        _moveCoroutine = null;
        
        TargetReached?.Invoke();
    }

    public void StopMove()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        
        _isMoving = false;
    }
}