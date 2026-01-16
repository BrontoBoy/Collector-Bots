using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InputReader))]
public class Game : MonoBehaviour
{
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private List<Castle> _castles = new List<Castle>();

    private InputReader _inputReader;
    
    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
    }
    
    private void Start()
    {
        StartResourceSpawning();
    }
    
    private void StartResourceSpawning()
    {
        if (_resourcesSpawner != null)
            _resourcesSpawner.StartSpawning();
    }
}
