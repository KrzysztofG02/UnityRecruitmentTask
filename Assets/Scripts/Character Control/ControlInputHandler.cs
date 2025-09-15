using System;
using UnityEngine;


public class ControlInputHandler : MonoBehaviour
{
    protected PlayerControls controls;

    public Vector2 moveInput { get; protected set; }
    public float rotateInput { get; protected set; }
    public event Action onInteract;

    protected virtual void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => this.moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => this.moveInput = Vector2.zero;

        controls.Player.Rotate.performed += ctx => this.rotateInput = ctx.ReadValue<float>();
        controls.Player.Rotate.canceled += _ => this.rotateInput = 0f;

        controls.Player.Interact.performed += _ => this.onInteract?.Invoke();
    }

    protected virtual void OnEnable() => controls.Enable();
    protected virtual void OnDisable() => controls.Disable();
}