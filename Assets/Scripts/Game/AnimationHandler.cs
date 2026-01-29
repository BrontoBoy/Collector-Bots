using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private string _speedParameter = "Speed";
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void SetMoving(bool isMoving)
    {
        if (_animator != null)
            _animator.SetFloat(_speedParameter, isMoving ? 1f : 0f);
    }
}