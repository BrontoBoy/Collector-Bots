using UnityEngine;

public class Unit : Entity, IPoolable
{
    [SerializeField] protected Mover Mover;
    
    protected Castle Castle;
}
