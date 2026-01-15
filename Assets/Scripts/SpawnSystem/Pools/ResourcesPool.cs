using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ResourcesPool : GameObjectsPool<Resource>
{
    public event System.Action<Resource> ResourceReturnedToPool;
    
    public override void ReturnObject(Resource item)
    {
        base.ReturnObject(item);
        ResourceReturnedToPool?.Invoke(item);
    }
}