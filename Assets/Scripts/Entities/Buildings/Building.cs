using UnityEngine;

public class Building : Entity, IPoolable, ITargetable
{
    public Vector3 Position => transform.position;
}