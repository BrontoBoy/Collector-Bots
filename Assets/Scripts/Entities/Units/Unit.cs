using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour, IPoolable
{
    [SerializeField] protected int Cost;
    
    protected Mover Mover; 
    protected Animator Animator;
    
    protected virtual void Awake()
    {
        Mover = GetComponent<Mover>();
        Animator = GetComponent<Animator>();
    }
}