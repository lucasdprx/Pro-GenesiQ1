using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMoveInput;
    public static event Action<Vector2> OnLookInput;
    public static event Action<Vector2> OnScrollInput;
    public static event Action OnMouseLeftClickPressed;
    public static event Action OnMouseLeftClickReleased;
    public static event Action OnMouseRightClickPressed;
    public static event Action OnMouseRightClickReleased;
    
    private bool isPointerOverGameObject;

    private void Update()
    {
        isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
    }

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
    
    public void MouseLeftClickInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isPointerOverGameObject)
        {
            OnMouseLeftClickPressed?.Invoke();
        }
        else if (ctx.canceled)
        {
            OnMouseLeftClickReleased?.Invoke();
        }
    }
    public void MouseRightClickInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isPointerOverGameObject)
        {
            OnMouseRightClickPressed?.Invoke();
        }
        else if (ctx.canceled)
        {
            OnMouseRightClickReleased?.Invoke();
        }
    }
}