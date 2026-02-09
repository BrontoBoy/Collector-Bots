using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _delay = 1f;
    
    public event System.Action<Gold> GoldFound;
    
    private Coroutine _scanCoroutine;
    
    private void OnEnable()
    {
        StartScanning();
    }

    private void OnDisable()
    {
        StopScanning();
    }
    
    // Метод для ручного удаления золота из отслеживания
    public void RemoveGold(Gold gold)
    {
        // В новой архитектуре сканер только находит,
        // но оставляем метод для совместимости
    }
    
    public Gold GetNearestGold()
    {
        // В новой архитектуре не используется,
        // оставляем для совместимости
        return null;
    }
    
    private void StartScanning()
    {
        _scanCoroutine = StartCoroutine(ScanRoutine());
    }

    private void StopScanning()
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
            FindGold();
        }
    }
    
    private void FindGold()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        
        foreach (Collider collider in colliders)
        {
            if (collider == null)
                continue;

            if (collider.TryGetComponent(out Gold gold))
            {
                GoldFound?.Invoke(gold);
            }
        }
    }
}