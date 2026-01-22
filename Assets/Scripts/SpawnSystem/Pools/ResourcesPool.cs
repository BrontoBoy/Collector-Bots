public class ResourcesPool : GameObjectsPool<Resource>
{
    public event System.Action<Resource> ResourceReturnedToPool;
    
    public override void ReturnObject(Resource item)
    {
        if (item == null)
            return;
        
        base.ReturnObject(item);
        ResourceReturnedToPool?.Invoke(item);
    }
}