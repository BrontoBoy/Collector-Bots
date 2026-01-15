using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourcesSpawner : Spawner<Resource>
{ 
    public static ResourcesSpawner СommonResourcesSpawner { get; private set; }
    
    public event Action<Resource> ResourceSpawned;
    
    private void Awake()
    {
        СommonResourcesSpawner = this;
    }
    
    private void OnDestroy()
    {
        if (СommonResourcesSpawner == this)
            СommonResourcesSpawner = null;
    }
    
    protected override void OnObjectSpawned(Resource spawnedObject)
    {
        base.OnObjectSpawned(spawnedObject);
        ResourceSpawned?.Invoke(spawnedObject);
    }
    
    public void ReturnResource(Resource resource)
    {
        if (GameObjectsPool != null && resource != null)
            GameObjectsPool.ReturnObject(resource);
    }
}