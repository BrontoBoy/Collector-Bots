using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class Game : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ResourceHandler _resourceHandler;   // назначить в инспекторе
    [SerializeField] private ResourcesSpawner _resourcesSpawner; // назначить в инспекторе

    [Header("World")]
    [SerializeField] private List<Castle> _castles = new List<Castle>(); // все замки сцены, назначить в инспекторе

    private InputReader _inputReader;

    // Удалите или закомментируйте вызов InitializeCastleSubscriptions() из Awake
    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        // НЕ ВЫЗЫВАЕМ InitializeCastleSubscriptions() здесь — Castle.Awake может ещё не выполниться
    }

    // Переносим инициализацию в Start
    private void Start()
    {
        // Сначала подписываемся на сканеры и запускаем их
        InitializeCastleSubscriptions();

        // Затем запускаем спавн ресурсов
        if (_resourcesSpawner != null)
            _resourcesSpawner.StartSpawning();
    }

    private void OnDestroy()
    {
        UnsubscribeFromCastleScanners();
    }


    private void InitializeCastleSubscriptions()
    {
        if (_castles == null || _castles.Count == 0)
            return;

        // Сначала подписываемся на события всех сканеров
        for (int i = 0; i < _castles.Count; i++)
        {
            Castle castle = _castles[i];
            if (castle == null)
                continue;

            Scanner scanner = castle.Scanner;
            if (scanner == null)
                continue;

            scanner.ResourceFoundAtPosition += OnScannerResourceFound;
        }

        // Затем запускаем сканирование у всех сканеров
        for (int i = 0; i < _castles.Count; i++)
        {
            Castle castle = _castles[i];
            if (castle == null)
                continue;

            Scanner scanner = castle.Scanner;
            if (scanner == null)
                continue;

            scanner.StartScanning();
        }
    }

    private void UnsubscribeFromCastleScanners()
    {
        if (_castles == null || _castles.Count == 0)
            return;

        for (int i = 0; i < _castles.Count; i++)
        {
            Castle castle = _castles[i];
            if (castle == null)
                continue;

            Scanner scanner = castle.Scanner;
            if (scanner == null)
                continue;

            scanner.ResourceFoundAtPosition -= OnScannerResourceFound;
        }
    }

    // Вызывается сканером (через событие)
    private void OnScannerResourceFound(Vector3 position)
    {
        if (_resourceHandler == null)
            return;

        // Добавляем ресурс в общий обработчик (ResourceHandler)
        _resourceHandler.AddResourceAtPosition(position);

        // Пытаемся назначить ближайший свободный рабочий из всех замков
        TryAssignNearestWorker(position);
    }

    private void TryAssignNearestWorker(Vector3 resourcePosition)
    {
        if (_castles == null || _castles.Count == 0)
            return;

        float bestDistance = float.MaxValue;
        Castle bestCastle = null;

        for (int i = 0; i < _castles.Count; i++)
        {
            Castle castle = _castles[i];
            if (castle == null)
                continue;

            WorkerHandler wh = castle.WorkerHandler;
            if (wh == null || wh.HasFreeWorkers == false)
                continue;

            float dist = Vector3.Distance(castle.transform.position, resourcePosition);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestCastle = castle;
            }
        }

        if (bestCastle == null)
            return;

        // Получаем ближайший ресурс относительно выбранного замка
        Resource nearest = _resourceHandler.GetNearestResource(bestCastle.transform.position);
        if (nearest == null)
            return;

        // Попросим замок назначить рабочего
        bool assigned = bestCastle.TryAssignWorkerToResource(nearest);

        // Если назначение успешно — удаляем ресурс из общего списка
        if (assigned)
            _resourceHandler.RemoveResource(nearest);
    }

    // Публичный доступ к ResourceHandler (если нужен)
    public ResourceHandler ResourceHandler => _resourceHandler;
}
