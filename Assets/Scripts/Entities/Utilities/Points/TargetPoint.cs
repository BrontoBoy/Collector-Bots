using UnityEngine;

public class TargetPoint : MonoBehaviour, ITargetable
{
    public Vector3 Position => transform.position;
}