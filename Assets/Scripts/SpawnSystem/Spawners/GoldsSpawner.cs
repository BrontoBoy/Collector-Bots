using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GoldsPool))]
public class GoldsSpawner : Spawner<Gold>
{ 
   [SerializeField] private float _delay = 2f;
    
    private Coroutine _spawnCoroutine;
    
    private void OnDisable()
    {
        StopSpawning();
    }
    
    public void StartSpawning()
    {
        StopSpawning();
        _spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
    }
    
    public void ReturnGold(Gold gold)
    {
        if (Pool != null && gold != null)
            Pool.ReturnObject(gold);
    }
    
    private IEnumerator AutoSpawnRoutine()
    {
        var wait = new WaitForSeconds(_delay);

        while (enabled)
        {
            SpawnRandom();
            yield return wait;
        }
    }
    
    private void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }
}