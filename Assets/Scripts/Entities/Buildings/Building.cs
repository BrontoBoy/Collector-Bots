using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour, IPoolable, ITargetable
{
    public Vector3 Position => transform.position;
}