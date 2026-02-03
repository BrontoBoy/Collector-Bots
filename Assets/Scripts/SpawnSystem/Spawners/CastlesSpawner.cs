using UnityEngine;

public class CastlesSpawner : Spawner<Castle> // OK: без пула
{
    [SerializeField] private Castle _castlePrefab;

    public Castle SpawnCastle(Vector3 position)
    {
        if (_castlePrefab == null)
            return null;

        Castle castle = CreateInstance(_castlePrefab); // CHANGED (Instantiate)
        castle.transform.position = position;

        OnObjectSpawned(castle);
        return castle;
    }
}