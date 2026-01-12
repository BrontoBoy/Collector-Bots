using UnityEngine;
using System.Collections;

public abstract class Spawner<T> : MonoBehaviour, ISpawner where T : MonoBehaviour
{
    /* protected const float ZeroZValue = 0f;
    
    private const float DefaultSpawnCooldown = 2f;
    
    [SerializeField] protected float SpawnRate = DefaultSpawnCooldown;
    [SerializeField] protected GameObjectPool<T> ObjectPool;
    
    protected bool IsSpawningActive = false;
    
    private Coroutine _spawningCoroutine;
    
    public System.Action<T> ObjectSpawned { get; set; }

    protected virtual void Start()
    {
        Initialize();
    }
    
    public virtual void Initialize()
    {
        StopSpawning();
    }
    
    public virtual void StartSpawning()
    {
        if (UseAutoSpawn == false || ObjectPool == null || IsSpawningActive)
            return;
        
        IsSpawningActive = true;
        
        _spawningCoroutine = StartCoroutine(SpawnRoutine());
    }
    
    public virtual void StopSpawning()
    {
        IsSpawningActive = false;
        
        if (_spawningCoroutine != null)
        {
            StopCoroutine(_spawningCoroutine);
            _spawningCoroutine = null;
        }
    }
    
    public void ReturnAllObjects()
    {
        if (ObjectPool != null)
            ObjectPool.ReturnAll();
    }
    
    public virtual T SpawnAtPosition(Vector3 position, Vector2 direction, Quaternion rotation)
    {
        if (ObjectPool == null)
            return null;
        
        T spawnedObject = ObjectPool.GetObject();
        
        if (spawnedObject != null)
        {
            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = rotation;
            
            InitializeObject(spawnedObject, direction);
            NotifyObjectSpawned(spawnedObject);
        }
        
        return spawnedObject;
    }
    
    protected virtual void SpawnAutomatically()
    {
        if (CanSpawn() == false)
            return;
        
        T spawnedObject = CreateObject();
        Vector3 spawnPosition = GetRandomSpawnPosition();
        spawnedObject.transform.position = spawnPosition;
        
        InitializeObject(spawnedObject);
        NotifyObjectSpawned(spawnedObject);
    }
    
    protected virtual T CreateObject()
    {
        return ObjectPool.GetObject();
    }
    
    protected virtual void InitializeObject(T gameObject, Vector2 direction = default) { }
    
    protected virtual void NotifyObjectSpawned(T spawnedObject)
    {
        ObjectSpawned?.Invoke(spawnedObject);
    }
    
    protected virtual bool CanSpawn()
    {
        return AreSpawnSettingsValid();
    }
    
    protected virtual Vector3 GetRandomSpawnPosition()
    {
        float randomY = GetRandomYPosition();
        
        return new Vector3(SpawnX, randomY, ZeroZValue);
    }
    
    protected float GetRandomYPosition()
    {
        return Random.Range(MinY, MaxY);
    }
    
    protected bool AreSpawnSettingsValid()
    {
        bool isMinYValid = MinY < MaxY;
        bool isSpawnRateValid = SpawnRate > ZeroZValue;
        bool isObjectPoolSet = ObjectPool != null;
        
        return isMinYValid && isSpawnRateValid && isObjectPoolSet;
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (IsSpawningActive && UseAutoSpawn && ObjectPool != null)
        {
            if (CanSpawn())
                SpawnAutomatically();
                
            yield return new WaitForSeconds(SpawnRate);
        }
    }*/
}