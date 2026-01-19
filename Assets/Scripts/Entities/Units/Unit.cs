using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Unit : MonoBehaviour, IPoolable
{
    [SerializeField] protected int Cost;
    
    public int UnitCost => Cost;
    
    protected Mover Mover; 
    protected Animator Animator;
	protected Collider Collider;
	protected Rigidbody Rigidbody;
    
    protected virtual void Awake()
    {
        Mover = GetComponent<Mover>();
        Animator = GetComponent<Animator>();
		Collider = GetComponent<Collider>();
		Rigidbody = GetComponent<Rigidbody>();
    }
}