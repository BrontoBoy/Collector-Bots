using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)] private float _speed = 5f;

    private Coroutine _coroutine;

    public void StartMove(Vector3 target)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(Move(target));
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        Vector3 currentTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        
        while(enabled)
        {
            transform.LookAt(currentTargetPosition);
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, _speed * Time.deltaTime);

            yield return null;
        }
    }

    public void StopMove()
    {
        StopCoroutine(_coroutine);
    }
}