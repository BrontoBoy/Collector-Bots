using UnityEngine;

public class GoldFactory : Factory<Gold>
{
    [SerializeField] private GoldsPool _goldsPool;

    public override Gold Create()
    {
        return _goldsPool.GetObject();
    }

    public override Gold Create(Vector3 position)
    {
        Gold gold = Create();
        gold.transform.position = position;
        
        return gold;
    }

    public void Release(Gold gold)
    {
        _goldsPool.ReturnObject(gold);
    }
}