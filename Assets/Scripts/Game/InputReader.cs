using UnityEngine;
using System;

public class InputReader : MonoBehaviour
{
    private const int SelectActionButtonIndex = 0;
    private const int CommandActionButtonIndex = 1;

    [SerializeField] private LayerMask _castleLayer;
    [SerializeField] private LayerMask _groundLayer;

    public event Action<Castle> CastleSelected;
    public event Action<Vector3> GroundSelected;
    public event Action DeselectRequested;

    private void Update()
    {
        if (Input.GetMouseButtonDown(SelectActionButtonIndex))
            HandleSelectAction();

        if (Input.GetMouseButtonDown(CommandActionButtonIndex))
            HandleCommandAction();
    }

    private void HandleSelectAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _castleLayer))
        {
            if (hit.collider.TryGetComponent(out Castle castle))
            {
                CastleSelected?.Invoke(castle);
                
                return;
            }
        }

        DeselectRequested?.Invoke();
    }

    private void HandleCommandAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            GroundSelected?.Invoke(hit.point);
    }
}