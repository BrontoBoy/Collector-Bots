using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)] private float _speed = 5f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveTo(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        _rigidbody.linearVelocity = direction * _speed;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Stop()
    {
        _rigidbody.linearVelocity = Vector3.zero; 
    }
}