using UnityEngine;

public class FlagFactory : Factory<Flag>
{
    [SerializeField] private FlagsPool _flagsPool;

    public override Flag Create()
    {
        return _flagsPool.GetObject();
    }

    public override Flag Create(Vector3 position)
    {
        Flag flag = Create();
        flag.transform.position = position;
        
        return flag;
    }

    public void Release(Flag flag)
    {
        _flagsPool.ReturnObject(flag);
    }
}