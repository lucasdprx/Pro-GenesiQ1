public class SelectorAbility : Ability
{
    private StateMachine currentHuman;
    public override void Capacity()
    {
        base.Capacity();
        if (hit.transform != null && hit.transform.TryGetComponent(out StateMachine human))
        {
            currentHuman = human;
            currentHuman.HandleSelection();
        }
    }

    private void OnDisable()
    {
        currentHuman = null;
    }
}
