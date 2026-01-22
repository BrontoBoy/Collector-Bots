using UnityEngine;

public class MarkersSpawner : Spawner<Marker>
{
    protected new void OnEnable()
    {
        AutoStart = false;
        base.OnEnable();
    }
    
    public Marker SpawnAtPosition(Vector3 position)
    {
        if (Pool != null)
        {
            Marker marker = Pool.GetObject();
            
            if (marker != null)
            {
                marker.transform.position = position;
                return marker;
            }
        }
        return null;
    }
}