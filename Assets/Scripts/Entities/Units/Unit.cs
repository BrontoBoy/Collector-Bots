using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour, IPoolable
{
    protected Mover Mover; 
    protected AnimationHandler AnimationHandler;
    
    protected virtual void Awake()
    {
        Mover = GetComponent<Mover>();
        AnimationHandler = GetComponent<AnimationHandler>();
    }
}