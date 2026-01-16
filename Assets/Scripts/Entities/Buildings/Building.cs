using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour, IPoolable, ITargetable
{
	[SerializeField] protected int Cost;

    public Vector3 Position => transform.position;
}