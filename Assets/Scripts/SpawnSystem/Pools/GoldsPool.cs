using UnityEngine;

public class GoldsPool : GameObjectsPool<Gold>
{
    protected override void OnReturnedToPool(Gold item)
    {
        base.OnReturnedToPool(item);
        item.GetComponent<Collider>().enabled = true;  // ← НОВОЕ: Enable collider для reuse
    }
}