using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour, IPoolable
{
    protected Mover Mover; 
    protected Animator Animator;
    
    protected virtual void Awake()
    {
        Mover = GetComponent<Mover>();
        Animator = GetComponent<Animator>();
    }
}