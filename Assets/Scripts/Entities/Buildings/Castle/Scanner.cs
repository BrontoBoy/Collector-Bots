using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _delay = 1f;

    public event System.Action<Vector3> ResourceFoundAtPosition;

    private Coroutine _scanCoroutine;

    // Запуск сканирования вызывается явно из Game после подписки
    public void StartScanning()
    {
        if (_scanCoroutine == null)
            _scanCoroutine = StartCoroutine(ScanRoutine());
    }

    public void StopScanning()
    {
        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }
    }

    private IEnumerator ScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        while (enabled)
        {
            yield return wait;
            FindResources();
        }
    }

    // Публичный метод, который делает OverlapSphere и вызывает событие с позицией ресурса
    public void FindResources()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        int count = colliders.Length;
        for (int i = 0; i < count; i++)
        {
            Collider collider = colliders[i];
            if (collider == null)
                continue;

            Resource resource;
            if (collider.TryGetComponent(out resource))
            {
                if (resource.IsScanned == false && resource.IsCollected == false)
                {
                    resource.MarkAsScanned();
                    ResourceFoundAtPosition?.Invoke(resource.transform.position);
                }
            }
        }
    }
}