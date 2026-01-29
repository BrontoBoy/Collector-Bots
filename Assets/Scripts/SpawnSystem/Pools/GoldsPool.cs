public class GoldsPool : GameObjectsPool<Gold>
{
    public event System.Action<Gold> ResourceReturnedToPool;
    
    public override void ReturnObject(Gold item)
    {
        if (item == null)
            return;
        
        base.ReturnObject(item);
        ResourceReturnedToPool?.Invoke(item);
    }
}