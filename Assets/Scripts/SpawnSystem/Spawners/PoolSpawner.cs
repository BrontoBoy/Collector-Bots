using UnityEngine;

public abstract class PoolSpawner<T> : Spawner<T> where T : MonoBehaviour
{
    [SerializeField] protected GameObjectsPool<T> Pool;

    public virtual void ReturnToPool(T item)
    {
        if (item != null && Pool != null)
            Pool.ReturnObject(item);
    }
    
    protected override T CreateInstance(T prefab)
    {
        return Pool != null ? Pool.GetObject() : base.CreateInstance(prefab);
    }
}