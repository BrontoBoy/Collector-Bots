using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InputReader))]
public class Game : MonoBehaviour
{
    [SerializeField] private ResourcesSpawner _resourcesSpawner;
    [SerializeField] private List<Castle> _castles = new List<Castle>();

    private InputReader _inputReader;
    
    public static event Action<Resource> ResourceDeliveredToStorage;
    
    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
    }
    
    private void Start()
    {
        StartResourceSpawning();
    }
    
    private void OnEnable()
    {
        ResourceDeliveredToStorage += OnResourceDeliveredToStorage;
    }
    
    private void OnDisable()
    {
        ResourceDeliveredToStorage -= OnResourceDeliveredToStorage;
    }
    
    private void StartResourceSpawning()
    {
        if (_resourcesSpawner != null)
            _resourcesSpawner.StartSpawning();
    }
    
    public static void NotifyResourceDelivered(Resource resource)
    {
        ResourceDeliveredToStorage?.Invoke(resource);
    }
    
    private void OnResourceDeliveredToStorage(Resource resource)
    {
        if (_resourcesSpawner != null)
        {
            _resourcesSpawner.ReturnResource(resource);
        }
    }
}