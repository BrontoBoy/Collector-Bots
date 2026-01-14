using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Storage))]
[RequireComponent(typeof(ResourceHandler))]
public class Castle : Building
{
    [SerializeField] private Storage _storage;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private ResourceHandler _resourceHandler;
    
    private void Awake()
    {
        _scanner = GetComponent<Scanner>();
        _storage = GetComponent<Storage>();
        _resourceHandler = GetComponent<ResourceHandler>();
    }

    private void OnEnable()
    {
        _scanner.ResourceFound += OnResourceFound;
    }

    private void OnDisable()
    {
        _scanner.ResourceFound -= OnResourceFound;
    }

    private void OnResourceFound(Resource resource)
    {
        _resourceHandler.AddResource(resource);
    }
}