using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action<Vector2> OnLookInput;
    public static event Action<Vector2> OnScrollInput; 

    public void Move(InputAction.CallbackContext ctx)
    {
        OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
    }
    public void Look(InputAction.CallbackContext ctx)
    {
        OnLookInput?.Invoke(ctx.ReadValue<Vector2>());
    }
    public void Scroll(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OnScrollInput?.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}