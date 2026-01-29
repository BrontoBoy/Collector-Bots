using TMPro;
using UnityEngine;

public class CastleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesText;

    public void UpdateGoldsDisplay(int resourcesCount)
    {
        if (_resourcesText != null)
            _resourcesText.text = $"{resourcesCount}";
    }
}