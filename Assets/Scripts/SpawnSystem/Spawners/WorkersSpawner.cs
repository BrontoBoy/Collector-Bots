using Random = UnityEngine.Random;

public class WorkersSpawner : Spawner<Worker>
{
    public Worker SpawnWorker()
    {
        if (Pool != null && SpawnPointsList != null && SpawnPointsList.Count > 0)
        {
            Worker worker = Pool.GetObject();
            
            if (worker != null)
            {
                int randomIndex = Random.Range(0, SpawnPointsList.Count);
                worker.transform.position = SpawnPointsList[randomIndex].transform.position;
                return worker;
            }
        }
        
        return null;
    }
}