using UnityEngine;

public class FlagsSpawner : PoolSpawner<Flag> // CHANGED: теперь PoolSpawner
{
    [SerializeField] private Flag _flagPrefab; // CHANGED

    public Flag SpawnAtPosition(Vector3 position)
    {
        if (_flagPrefab == null)
            return null;

        Flag flag = CreateInstance(_flagPrefab); // CHANGED: из пула
        flag.transform.position = position;

        return flag;
    }
}