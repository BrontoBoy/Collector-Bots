using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Gold : MonoBehaviour, ITargetable
{
    public Vector3 Position => transform.position;
}