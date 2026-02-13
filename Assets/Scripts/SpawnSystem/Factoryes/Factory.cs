using UnityEngine;

public abstract class Factory<T> : MonoBehaviour, IFactory<T> where T : MonoBehaviour
{
    [SerializeField] protected T Prefab;

    public virtual T Create()
    {
        return Instantiate(Prefab);
    }

    public virtual T Create(Vector3 position)
    {
        T instance = Create();
        instance.transform.position = position;
        
        return instance;
    }
}