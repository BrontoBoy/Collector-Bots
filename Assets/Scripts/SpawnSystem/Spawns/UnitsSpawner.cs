using Random = UnityEngine.Random;

public class UnitsSpawner : Spawner<Unit>
{
    protected new void OnEnable()
    {
        AutoStart = false;
        base.OnEnable();
    }
    
    public Unit SpawnWorker()
    {
        if (Pool != null && SpawnPointsList != null && SpawnPointsList.Count > 0)
        {
            Unit unit = Pool.GetObject();
            
            if (unit != null)
            {
                int randomIndex = Random.Range(0, SpawnPointsList.Count);
                unit.transform.position = SpawnPointsList[randomIndex].transform.position;
                return unit;
            }
        }
        return null;
    }
}