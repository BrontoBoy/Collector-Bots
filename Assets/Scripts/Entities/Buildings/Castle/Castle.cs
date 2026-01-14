using UnityEngine;

public class Castle : Building
{
    [SerializeField] private Storage _storage;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private ResourceHandler _resourceHandler;
}