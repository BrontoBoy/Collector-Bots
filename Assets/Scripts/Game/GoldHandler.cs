using System;
using System.Collections.Generic;
using UnityEngine;

public class GoldHandler : MonoBehaviour
{
    private List<Gold> _availableGolds = new List<Gold>();
    private List<Gold> _assignedGolds = new List<Gold>(); 
    
    public event Action<Gold> GoldReadyForCollection;
    public event Action<Gold> GoldAssigned;
    public event Action<Gold> GoldReleased;
    public event Action<Gold> GoldCollected; 
    
    public bool TryAddGold(Gold gold)
    {
        if (gold == null)
            return false;

        if (_availableGolds.Contains(gold) || _assignedGolds.Contains(gold))
            return false;
        
        _availableGolds.Add(gold);
        
        GoldReadyForCollection?.Invoke(gold);
        
        return true;
    }
    
    public Gold GetFreeGold()
    {
        if (_availableGolds.Count == 0)
            return null;
            
        return _availableGolds[0];
    }
    
    public bool MarkAsAssigned(Gold gold)
    {
        if (gold == null || _availableGolds.Contains(gold) == false)
            return false;
            
        _availableGolds.Remove(gold);
        _assignedGolds.Add(gold);
        
        GoldAssigned?.Invoke(gold);
        
        return true;
    }
    
    public bool ReleaseGold(Gold gold)
    {
        if (gold == null || _assignedGolds.Contains(gold) == false)
            return false;
            
        _assignedGolds.Remove(gold);
        _availableGolds.Add(gold);
        
        GoldReleased?.Invoke(gold);
        GoldReadyForCollection?.Invoke(gold);
        
        return true;
    }

    public void RemoveGold(Gold gold)
    {
        if (gold == null)
            return;
        
        if (_availableGolds.Contains(gold))
            _availableGolds.Remove(gold);
            
        if (_assignedGolds.Contains(gold))
            _assignedGolds.Remove(gold);

        GoldCollected?.Invoke(gold);
    }
}