using UnityEngine;

public class Resource : Entity, IPoolable, IScannable, ITargetable
{
    public Vector3 Position => transform.position;
}