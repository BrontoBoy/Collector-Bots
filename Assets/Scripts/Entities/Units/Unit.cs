using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Unit : Entity, IPoolable
{
    [SerializeField] protected int Cost;
    
    protected Mover Mover;
    
    protected virtual void Awake()
    {
        Mover = GetComponent<Mover>();
    }
}