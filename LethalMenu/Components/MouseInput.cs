using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Components;

internal class MouseInput : MonoBehaviour
{
    private float Pitch;
    private float Yaw;

    private void Update()
    {
        if (Cursor.visible) return;

        Yaw += Mouse.current.delta.x.ReadValue() * Settings.f_mouseSensitivity;
        Yaw = (Yaw + 360) % 360;

        Pitch -= Mouse.current.delta.y.ReadValue() * Settings.f_mouseSensitivity;
        Pitch = Mathf.Clamp(Pitch, -90, 90);

        transform.eulerAngles = new Vector3(Pitch, Yaw, 0f);
    }
}