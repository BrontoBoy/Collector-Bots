using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    private readonly List<Resource> _resources = new List<Resource>();

    // Добавляет ресурс по позиции (ищет коллайдеры вокруг позиции)
    public void AddResourceAtPosition(Vector3 position, float searchRadius = 0.5f)
    {
        Collider[] colliders = Physics.OverlapSphere(position, searchRadius);
        if (colliders == null || colliders.Length == 0)
            return;

        int count = colliders.Length;
        for (int i = 0; i < count; i++)
        {
            Collider c = colliders[i];
            if (c == null)
                continue;

            Resource resource;
            if (c.TryGetComponent(out resource))
            {
                if (resource != null && resource.IsCollected == false && _resources.Contains(resource) == false)
                {
                    _resources.Add(resource);
                }
                // Обрабатываем первый найденный ресурс и выходим
                return;
            }
        }
    }

    // Удаляет ресурс из списка (например, при назначении рабочего)
    public void RemoveResource(Resource resource)
    {
        if (resource == null)
            return;

        _resources.Remove(resource);
    }

    // Возвращает ближайший ресурс относительно позиции
    public Resource GetNearestResource(Vector3 referencePosition)
    {
        Resource best = null;
        float bestDist = float.MaxValue;

        int count = _resources.Count;
        for (int i = 0; i < count; i++)
        {
            Resource r = _resources[i];
            if (r == null || r.IsCollected)
                continue;

            float dist = Vector3.Distance(referencePosition, r.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = r;
            }
        }

        return best;
    }

    // Возврат ресурса в пул и удаление из списка
    public void ReturnResourceToPool(Resource resource)
    {
        if (resource == null)
            return;

        _resources.Remove(resource);
        resource.ResetState();
        ResourcesSpawner.CommonResourcesSpawner?.ReturnResource(resource);
    }

    // Для отладки: снимок текущего списка
    public List<Resource> GetAllResourcesSnapshot()
    {
        return new List<Resource>(_resources);
    }
}
