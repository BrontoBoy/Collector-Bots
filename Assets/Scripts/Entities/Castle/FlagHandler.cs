using UnityEngine;

[RequireComponent(typeof(FlagsSpawner))]
public class FlagHandler : MonoBehaviour
{
    private FlagsSpawner _flagsSpawner;
    private Flag _flag;
    private bool _hasFlag = false;
    
    public bool HasFlag => _hasFlag;
    public Flag Flag => _flag;

    private void Awake()
    {
        _flagsSpawner = GetComponent<FlagsSpawner>();
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_flagsSpawner == null)
            return;

        if (_hasFlag == false)
        {
            _flag = _flagsSpawner.SpawnAtPosition(position);
            _hasFlag = true;
        }
        else if (_flag != null)
        {
            _flag.transform.position = position;
        }
    }

    public void RemoveFlag()
    {
        if (_hasFlag && _flag != null)
        {
            _flagsSpawner.ReturnToPool(_flag); // CHANGED
            _flag = null;
            _hasFlag = false;
        }
    }
}