using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CastlesHandler : MonoBehaviour
{
    [SerializeField] private List<Castle> _castles = new List<Castle>();
    
    public IReadOnlyList<Castle> Castles => _castles.AsReadOnly();
    
    public Castle GetNearestCastle(Vector3 goldPosition)
    {
        if (_castles == null || _castles.Count == 0)
            return null;
    
        Castle nearestCastle;
    
        if (_castles.Count == 1)
        {
            nearestCastle = _castles[0];
            
            if (nearestCastle != null && nearestCastle.WorkerHandler != null && nearestCastle.WorkerHandler.FreeWorkersCount > 0)
                return nearestCastle;
        
            return null;
        }
        
        nearestCastle = _castles
            .Where(castle => castle != null && castle.WorkerHandler != null && castle.WorkerHandler.FreeWorkersCount > 0)
            .OrderBy(castle => Vector3.Distance(goldPosition, castle.transform.position))
            .FirstOrDefault();
    
        return nearestCastle;
    }
}