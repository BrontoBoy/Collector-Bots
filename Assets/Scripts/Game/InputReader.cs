using UnityEngine;
using UnityEngine.Serialization;

public class InputReader : MonoBehaviour
{
    public const int LeftMouseButtonIndex = 0;
    public const int RightMouseButtonIndex = 1;
    
    [SerializeField] private LayerMask _castleLayer;
    [SerializeField] private LayerMask _groundLayer;
    
    private Castle _selectedCastle;
    
    public event System.Action<Castle> CastleSelected;
    public event System.Action<Castle> CastleDeselected;
    public event System.Action<Vector3> GroundRightClicked;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseButtonIndex))
            HandleLeftClick();
        
        if (Input.GetMouseButtonDown(RightMouseButtonIndex))
            HandleRightClick();
    }
    
    private void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _castleLayer))
        {
            if (hit.collider.TryGetComponent<Castle>(out Castle castle))
                SelectCastle(castle);
        }
        else
        {
            DeselectCurrentCastle();
        }
    }
    
    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            GroundRightClicked?.Invoke(hit.point);
    }
    
    private void SelectCastle(Castle castle)
    {
        DeselectCurrentCastle();
        _selectedCastle = castle;
        castle.Select();
        CastleSelected?.Invoke(castle);
    }
    
    private void DeselectCurrentCastle()
    {
        if (_selectedCastle != null)
        {
            CastleDeselected?.Invoke(_selectedCastle);
            
            _selectedCastle.Deselect();
            _selectedCastle = null;
        }
    }
}