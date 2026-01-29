using UnityEngine;

public class FlagsSpawner : Spawner<Flag>
{
    public Flag SpawnAtPosition(Vector3 position)
    {
        if (Pool != null)
        {
            Flag flag = Pool.GetObject();
            
            if (flag != null)
            {
                flag.transform.position = position;
                return flag;
            }
        }
        return null;
    }
}