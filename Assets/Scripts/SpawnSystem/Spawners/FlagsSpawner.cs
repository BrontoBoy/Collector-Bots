using UnityEngine;

public class FlagsSpawner : PoolSpawner<Flag>
{
    [SerializeField] private Flag _flagPrefab;

    public Flag SpawnAtPosition(Vector3 position)
    {
        if (_flagPrefab == null)
            return null;

        Flag flag = CreateInstance(_flagPrefab);
        flag.transform.position = position;

        return flag;
    }
}