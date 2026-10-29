using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>Adapta controles al Input Manager clásico o Input System en Unity 6.</summary>
public static class MVKeys
{
    public static bool Held(KeyCode key)
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(key);
#elif ENABLE_INPUT_SYSTEM
        Key k = Translate(key);
        return Keyboard.current != null && k != Key.None && Keyboard.current[k].isPressed;
#else
        return false;
#endif
    }

    public static bool Pressed(KeyCode key)
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(key);
#elif ENABLE_INPUT_SYSTEM
        Key k = Translate(key);
        return Keyboard.current != null && k != Key.None && Keyboard.current[k].wasPressedThisFrame;
#else
        return false;
#endif
    }

#if ENABLE_INPUT_SYSTEM
    private static Key Translate(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.RightArrow: return Key.RightArrow;
            case KeyCode.LeftArrow: return Key.LeftArrow;
            case KeyCode.UpArrow: return Key.UpArrow;
            case KeyCode.A: return Key.A;
            case KeyCode.D: return Key.D;
            case KeyCode.W: return Key.W;
            case KeyCode.Space: return Key.Space;
            case KeyCode.Escape: return Key.Escape;
            default: return Key.None;
        }
    }
#endif
}
