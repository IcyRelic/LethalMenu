using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Components;

internal class MouseInput : MonoBehaviour
{
    private float _pitch;
    private float _yaw;

    private void Update()
    {
        if (Cursor.visible) return;

        _yaw += Mouse.current.delta.x.ReadValue() * Settings.f_mouseSensitivity;
        _yaw = (_yaw + 360) % 360;

        _pitch -= Mouse.current.delta.y.ReadValue() * Settings.f_mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -90, 90);

        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}