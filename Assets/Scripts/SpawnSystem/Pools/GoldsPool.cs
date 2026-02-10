using UnityEngine;

public class GoldsPool : GameObjectsPool<Gold>
{
    protected override void OnTakeFromPool(Gold item)
    {
        base.OnTakeFromPool(item);
        
        Collider collider = item.GetComponent<Collider>();
        
        if (collider != null)
            collider.enabled = true;
    }
}