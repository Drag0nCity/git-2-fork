using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputService : IInputService
{
    private readonly DefaultInputActions _inputActions;

    public InputService()
    {
        _inputActions = new DefaultInputActions();
        _inputActions.Enable();
    }

    public Vector2 Move =>
        _inputActions.Player.Move.ReadValue<Vector2>();

    public Vector2 Look =>
        _inputActions.Player.Look.ReadValue<Vector2>();

    public bool FirePressed =>
        _inputActions.Player.Fire.WasPressedThisFrame();

    public bool AimPressed => false;

    public bool JumpPressed => false;

    public bool Crouch => false;

    public bool ReloadPressed =>
    Keyboard.current != null &&
    Keyboard.current.rKey.wasPressedThisFrame;

    public bool InteractPressed =>
    Keyboard.current != null &&
    Keyboard.current.eKey.wasPressedThisFrame;

    public int WeaponSlot
    {
        get
        {
            if (Keyboard.current == null)
            {
                return -1;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                return 0;

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                return 1;

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                return 2;

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
                return 3;

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
                return 4;

            return -1;
        }
    }

    public float WeaponScroll
    {
        get
        {
            if (Mouse.current == null)
            {
                return 0f;
            }

            return Mouse.current.scroll.ReadValue().y;
        }
    }
}