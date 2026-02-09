using UnityEngine;

[RequireComponent(typeof(FlagsSpawner))]
public class FlagHandler : MonoBehaviour
{
    private FlagsSpawner _flagsSpawner;
    
    public bool HasFlag { get; private set; }
    public Flag Flag { get; private set; }

    private void Awake()
    {
        _flagsSpawner = GetComponent<FlagsSpawner>();
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_flagsSpawner == null)
            return;

        if (HasFlag == false)
        {
            Flag = _flagsSpawner.SpawnAtPosition(position);
            HasFlag = true;
        }
        else if (Flag != null)
        {
            Flag.transform.position = position;
        }
    }

    public void RemoveFlag()
    {
        if (HasFlag && Flag != null)
        {
            _flagsSpawner.ReturnToPool(Flag);
            Flag = null;
            HasFlag = false;
        }
    }
}