using UnityEngine;

public class InputReader : MonoBehaviour
{
    private const int LeftMouseButtonIndex = 0;
    private const int RightMouseButtonIndex = 1;
    
    [SerializeField] private LayerMask _castleLayer;
    [SerializeField] private LayerMask _groundLayer;
    
    private Castle _selectedCastle;

    public event System.Action<Castle, Vector3> GroundRightClickedWithCastle;
    
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
		if (_selectedCastle == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            GroundRightClickedWithCastle?.Invoke(_selectedCastle, hit.point); 
    }
    
    private void SelectCastle(Castle castle)
    {
        DeselectCurrentCastle();
        _selectedCastle = castle;
        castle.Select();
    }
    
    private void DeselectCurrentCastle()
    {
        if (_selectedCastle != null)
        {
            _selectedCastle.Deselect();
            _selectedCastle = null;
        }
    }
}