using TMPro;
using UnityEngine;

public class CastleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesText;
    [SerializeField] private Storage _storage;
    
    private void Awake() 
    {
        UpdateGoldsDisplay(_storage.GoldsValue);
    }

    private void OnEnable()
    {
        _storage.GoldsChanged += UpdateGoldsDisplay;
    }

    private void OnDisable() 
    {
        _storage.GoldsChanged -= UpdateGoldsDisplay;
    }

    private void UpdateGoldsDisplay(int resourcesCount)
    {
        if (_resourcesText != null)
            _resourcesText.text = $"{resourcesCount}";
    }
}