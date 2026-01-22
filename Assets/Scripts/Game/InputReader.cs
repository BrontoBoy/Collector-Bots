using UnityEngine;

public class InputReader : MonoBehaviour
{
    public const int LeftMouseButtonIndex = 0;
    public const int RightMouseButtonIndex = 1;
    
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private LayerMask _groundLayer;
    
    private Building _selectedBuilding;
    
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
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _buildingLayer))
        {
            if (hit.collider.TryGetComponent<Building>(out Building building))
                SelectBuilding(building);
        }
        else
        {
            DeselectCurrentBuilding();
        }
    }
    
    private void HandleRightClick()
    {
        if (_selectedBuilding is Castle selectedCastle)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
                GroundRightClicked?.Invoke(hit.point);
        }
    }
    
    private void SelectBuilding(Building building)
    {
        DeselectCurrentBuilding();
        _selectedBuilding = building;
        building.Select();
        
        if (building is Castle castle)
            CastleSelected?.Invoke(castle);
    }
    
    private void DeselectCurrentBuilding()
    {
        if (_selectedBuilding != null)
        {
            if (_selectedBuilding is Castle castle)
                CastleDeselected?.Invoke(castle);
            
            _selectedBuilding.Deselect();
            _selectedBuilding = null;
        }
    }
}