using UnityEngine;

public class Marker : Entity, IPoolable
{
    public Vector3 Position => transform.position;
}