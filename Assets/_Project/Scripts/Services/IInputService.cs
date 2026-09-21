using UnityEngine;

public interface IInputService
{
    Vector2 Move { get; }
    Vector2 Look { get; }

    bool FirePressed { get; }
    bool AimPressed { get; }
    bool JumpPressed { get; }
    bool Crouch { get; }
    bool ReloadPressed { get; }
    bool InteractPressed { get; }

    int WeaponSlot { get; }
    float WeaponScroll { get; }

}