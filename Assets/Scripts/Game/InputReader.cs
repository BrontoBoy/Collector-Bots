using UnityEngine;

public class InputReader : MonoBehaviour
{
    private const int SelectActionButtonIndex = 0;
    private const int CommandActionButtonIndex = 1;
    
    [SerializeField] private LayerMask _castleLayer;
    [SerializeField] private LayerMask _groundLayer;
    
    private Castle _selectedCastle;

    public event System.Action<Castle, Vector3> CommandReceived;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(SelectActionButtonIndex))
            OnSelectAction();
        
        if (Input.GetMouseButtonDown(CommandActionButtonIndex))
            OnCommandAction();
    }
    
    private void OnSelectAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _castleLayer))
        {
            if (hit.collider.TryGetComponent(out Castle castle))
            	SelectCastle(castle);
        }
        else
        {
            DeselectCurrentCastle();
        }
    }
    
    private void OnCommandAction()
    {
		if (_selectedCastle == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            CommandReceived?.Invoke(_selectedCastle, hit.point); 
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