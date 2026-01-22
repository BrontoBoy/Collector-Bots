using System;
using UnityEngine;

public class ResourcesSpawner : Spawner<Resource>
{ 
    public static ResourcesSpawner CommonResourcesSpawner { get; private set; }
    
    public event Action<Resource> ResourceSpawned;
    
    private void Awake()
    {
        CommonResourcesSpawner = this;
    }
    
    private void OnDestroy()
    {
        if (CommonResourcesSpawner == this)
            CommonResourcesSpawner = null;
    }
    
    protected override void OnObjectSpawned(Resource spawnedObject)
    {
        base.OnObjectSpawned(spawnedObject);
        ResourceSpawned?.Invoke(spawnedObject);
    }
    
    public void ReturnResource(Resource resource)
    {
        if (GameObjectsPool != null && resource != null)
        {
            resource.ResetState();
            GameObjectsPool.ReturnObject(resource);
        }
    }
}