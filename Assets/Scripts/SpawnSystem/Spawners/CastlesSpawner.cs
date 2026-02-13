using System;
using UnityEngine;

public class CastlesSpawner : MonoBehaviour
{
    [SerializeField] private CastleFactory _castleFactory;

    public event Action<Castle> CastleSpawned;
    
    public Castle SpawnCastle(Vector3 position)
    {
        if (_castleFactory == null)
            return null;

        Castle newCastle = _castleFactory.Create(position);
        CastleSpawned?.Invoke(newCastle);
        
        return newCastle;
    }
    
    public void OnCastleSpawnRequested(Vector3 position, Action<Castle> Spawned)
    {
        Castle newCastle = SpawnCastle(position);
        Spawned?.Invoke(newCastle);
    }
}