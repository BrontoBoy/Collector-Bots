using UnityEngine;

public class Carrier : MonoBehaviour
{
    [SerializeField] private SpawnPoint _carryPoint;
    
    public Gold TargetGold { get; private set; }
    
    public void SetTargetGold(Gold gold)
    {
        TargetGold = gold;
    }

    public void AttachGold(Gold gold)
    {
        gold.transform.SetParent(_carryPoint.transform);
        gold.transform.localPosition = Vector3.zero;
        gold.transform.localRotation = Quaternion.identity;
    }
    
    public void DetachGold()
    {
    }
}