using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    private FlagFactory _flagFactory;

    public bool HasFlag { get; private set; }
    public Flag Flag { get; private set; }

    private void Awake()
    {
        _flagFactory = GetComponent<FlagFactory>();
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_flagFactory == null)
            return;

        if (HasFlag == false)
        {
            Flag = _flagFactory.Create(position);
            HasFlag = true;
        }
        else if (Flag != null)
        {
            Flag.transform.position = position;
        }
    }

    public void RemoveFlag()
    {
        if (HasFlag)
        {
            _flagFactory.Release(Flag);
            Flag = null;
            HasFlag = false;
        }
    }
}