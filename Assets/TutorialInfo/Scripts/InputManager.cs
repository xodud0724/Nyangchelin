using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool TapDown
    {
        get;
        private set;
    }

    public Vector2 TapPosition
    {
        get;
        private set;
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        TapDown = false;

        // 모바일 터치
        var touch = Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.wasPressedThisFrame)
        {
            TapDown = true;
            TapPosition = touch.primaryTouch.position.ReadValue();
        }

        // 마우스 클릭
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            TapDown = true;
            TapPosition = mouse.position.ReadValue();
        }
    }
}
