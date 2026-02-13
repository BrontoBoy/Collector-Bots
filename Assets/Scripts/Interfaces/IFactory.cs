using UnityEngine;

public interface IFactory<T> where T : MonoBehaviour
{
    T Create();
    T Create(Vector3 position);
}