using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    private Ability currentAbility;
    private bool inputLeftCLick;

    private void Awake()
    {
        InputManager.OnMouseLeftClickPressed += InputManagerOnMouseLeftClickPressed;
        InputManager.OnMouseLeftClickReleased += InputManagerOnMouseLeftClickRelease;
    }

    public void ChangeAbility(Ability newAbility)
    {
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
