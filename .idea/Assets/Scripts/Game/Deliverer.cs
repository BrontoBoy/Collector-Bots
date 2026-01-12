using UnityEngine;

public class Deliverer : MonoBehaviour
{
    private GoldenOre _targetGoldenOre;

    private void Awake()
    {
        _targetGoldenOre = GetComponentInChildren<GoldenOre>();
    }

    public void PickUp(GoldenOre goldenOre)
    {
        goldenOre.transform.parent = _targetGoldenOre.transform;
        goldenOre.transform.position = _targetGoldenOre.transform.position;
    }
}