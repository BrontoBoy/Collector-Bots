using UnityEngine;

public class Storage : MonoBehaviour
{
    private int _resourcesValue = 0;

    public int ResourcesValue => _resourcesValue;
        
    public void AddResource()
    {
        _resourcesValue++;
    }
    
    public void SpendResource(int price)
    {
        _resourcesValue -= price;
    }
}