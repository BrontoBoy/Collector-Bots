using UnityEngine;

public class Marker : MonoBehaviour, IPoolable
{
    public Vector3 Position => transform.position;
}