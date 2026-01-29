using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _delay = 1f;

    public event Action<Gold> GoldFound;
    
    private List<Gold> _foundGolds = new List<Gold>();
    private List<Gold> _assignedGolds = new List<Gold>();
    private Coroutine _scanCoroutine;
    
    public int FoundGoldsCount => _foundGolds.Count;

    private void OnDisable()
    {
        if (_scanCoroutine == null)
            StopScanning();
    }

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
            FindGold();
        }
    }
    
    private void SortGoldsByDistance()
    {
        Vector3 castlePosition = transform.position;
        
        _foundGolds = _foundGolds.Where(gold => gold != null)
            .OrderBy(gold => Vector3.Distance(castlePosition, gold.transform.position)).ToList();
    }
    
    public void FindGold()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        int count = colliders.Length;
        
        for (int i = 0; i < count; i++)
        {
            Collider collider = colliders[i];
            
            if (collider == null)
                continue;

            Gold gold;
            
            if (collider.TryGetComponent(out gold))
            {
                if (_assignedGolds.Contains(gold) || _foundGolds.Contains(gold))
                    continue;
    
                _foundGolds.Add(gold);
                SortGoldsByDistance();
                GoldFound?.Invoke(gold);
            }
        }
    }
    
    public Gold GetNearestGold()
    {
        Gold nearestGold = _foundGolds[0];
        _assignedGolds.Add(nearestGold);
        _foundGolds.RemoveAt(0);
        
        return nearestGold;
    }
    
    public void AddGold(Gold gold)
    {
        _foundGolds.Add(gold);
        SortGoldsByDistance(); 
    }
    
    public void RemoveGold(Gold gold)
    {
        _foundGolds.Remove(gold);
        _assignedGolds.Remove(gold);
    }
}