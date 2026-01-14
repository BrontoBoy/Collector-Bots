using UnityEngine;

public class Worker : Unit
{
    private Gold _currentGold;
    private bool _isFree;
    
    public void MoveToTarget(ITargetable target)
    {
        Mover.StartMove(target.Position);
    }
}