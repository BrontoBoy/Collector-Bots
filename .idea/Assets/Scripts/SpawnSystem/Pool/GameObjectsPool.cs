using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class GameObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T Prefab;
    [SerializeField] protected int InitialSize = 15;
    [SerializeField] protected int MaxSize = 100;
    
    protected ObjectPool<T> Pool;
    protected List<T> ActiveObjects = new List<T>();
    
    protected virtual void Awake()
    {
        if (Prefab == null)
            return;
        
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
    
    public virtual void ReturnAll()
    {
        List<T> activeCopy = new List<T>(ActiveObjects);
        
        foreach (T item in activeCopy)
        {
            if (item != null && item.gameObject.activeInHierarchy)
                ReturnObject(item);
        }
        
        ActiveObjects.Clear();
    }
    
    public virtual void ClearPool()
    {
        Pool.Clear();
        ActiveObjects.Clear();
    }
    
    protected virtual void InitializePool()
    {
        Pool = new ObjectPool<T>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: InitialSize,
            maxSize: MaxSize
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
        ActiveObjects.Add(item);
    }
    
    protected virtual void OnReturnedToPool(T item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);
        ActiveObjects.Remove(item);
    }
    
    protected virtual void OnDestroyPoolObject(T item)
    {
        Destroy(item.gameObject);
    }
    
    protected virtual void PrewarmPool()
    {
        List<T> items = new List<T>();
        
        for (int i = 0; i < InitialSize; i++)
        {
            T item = Pool.Get();
            items.Add(item);
        }
        
        foreach (T item in items)
        {
            Pool.Release(item);
        }
    }
}