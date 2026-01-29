using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class GameObjectsPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T Prefab;
    [SerializeField] protected int PoolCapacity = 15;
    [SerializeField] protected int MaxPoolCapacity = 100;
    
    protected ObjectPool<T> Pool;
    
    protected void OnValidate()
    {
        if (PoolCapacity > MaxPoolCapacity)
            PoolCapacity = MaxPoolCapacity - 1;
    }
    
    protected virtual void Awake()
    {
        InitializePool();
    }
    
    public virtual T GetObject()
    {
        return Pool.Get();
    }
    
    public virtual void ReturnObject(T item)
    {
        if (item == null)
            return;
        
        Pool.Release(item);
    }
    
    protected virtual void InitializePool()
    {
        Pool = new ObjectPool<T>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: PoolCapacity,
            maxSize: MaxPoolCapacity
        );
        
        PrewarmPool();
    }
    
    protected virtual T CreatePooledItem()
    {
        T newItem = Instantiate(Prefab);
        newItem.transform.SetParent(transform);
        newItem.gameObject.SetActive(false);
        
        return newItem;
    }
    
    protected virtual void OnTakeFromPool(T item)
    {
        item.transform.SetParent(null);
        item.gameObject.SetActive(true);
    }
    
    protected virtual void OnReturnedToPool(T item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);
    }
    
    protected virtual void OnDestroyPoolObject(T item)
    {
        Destroy(item.gameObject);
    }
    
    protected virtual void PrewarmPool()
    {
        List<T> items = new List<T>();
        
        for (int i = 0; i < PoolCapacity; i++)
        {
            T item = Pool.Get();
            items.Add(item);
        }
        
        foreach (T item in items)
            Pool.Release(item);
    }
}