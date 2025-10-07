using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    [SerializeField] private Ability defaultAbility;
    
    private Ability currentAbility;
    private bool inputLeftCLick;

    private void Awake()
    {
        InputManager.OnMouseLeftClickPressed += InputManagerOnMouseLeftClickPressed;
        InputManager.OnMouseLeftClickReleased += InputManagerOnMouseLeftClickRelease;
        if (defaultAbility)
            ChangeAbility(defaultAbility);
    }

    public void ChangeAbility(Ability newAbility)
    {
        if (newAbility == currentAbility) return;
        
        if (currentAbility)
            currentAbility.enabled = false;
        newAbility.enabled = true;
        currentAbility = newAbility;
    }

    private void Update()
    {
        if (currentAbility is null || !inputLeftCLick) return;
        
        if (currentAbility.inputHold)
        {
            currentAbility.Capacity();
        }
    }
    
    public void ClearAbility()
    {
        if (currentAbility)
            currentAbility.enabled = false;
        currentAbility = null;
    }
    
    private void InputManagerOnMouseLeftClickPressed()
    {
        inputLeftCLick = true;
        if (currentAbility is null) return;
        if (!currentAbility.inputHold)
            currentAbility.Capacity();
    }
    private void InputManagerOnMouseLeftClickRelease()
    {
        inputLeftCLick = false;
    }
}
