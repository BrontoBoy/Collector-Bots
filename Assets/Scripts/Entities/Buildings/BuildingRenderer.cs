using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BuildingRenderer : MonoBehaviour
{
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _clickedColor = Color.green;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        OnDefault();
    }

    public void OnDefault()
    {
        _renderer.material.color = _defaultColor;
    }

    public void OnClick()
    {
        _renderer.material.color = _clickedColor;
    }
}