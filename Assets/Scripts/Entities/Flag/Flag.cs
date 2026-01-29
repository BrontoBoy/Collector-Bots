using UnityEngine;

public class Flag : MonoBehaviour, ITargetable
{
    public Vector3 Position => transform.position;
}