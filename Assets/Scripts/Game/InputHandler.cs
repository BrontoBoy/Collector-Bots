using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class InputHandler : MonoBehaviour
{
    private InputReader _inputReader;
    private Castle _selectedCastle;

    private void Awake()
    {
        _inputReader = GetComponent<InputReader>();
    }

    private void OnEnable()
    {
        _inputReader.CastleSelected += OnCastleSelected;
        _inputReader.GroundSelected += OnGroundSelected;
        _inputReader.DeselectRequested += OnDeselectRequested;
    }

    private void OnDisable()
    {
        _inputReader.CastleSelected -= OnCastleSelected;
        _inputReader.GroundSelected -= OnGroundSelected;
        _inputReader.DeselectRequested -= OnDeselectRequested;
    }

    private void OnCastleSelected(Castle castle)
    {
        if (_selectedCastle != null && _selectedCastle != castle)
            _selectedCastle.Deselect();

        _selectedCastle = castle;
        castle.Select();
    }

    private void OnDeselectRequested()
    {
        if (_selectedCastle != null)
        {
            _selectedCastle.Deselect();
            _selectedCastle = null;
        }
    }

    private void OnGroundSelected(Vector3 position)
    {
        if (_selectedCastle == null)
            return;

        _selectedCastle.PlaceFlag(position);
    }
}