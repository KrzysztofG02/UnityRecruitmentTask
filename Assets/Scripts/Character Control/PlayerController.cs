using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : ControlInputHandler
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private string tooFarMessage = "You are too far from the object to interact.";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Camera Bob Settings")]
    [SerializeField] private float verticalBobFreq = 5f;
    [SerializeField] private float bobHeight = 0.2f;    
    [SerializeField] private float speedThreshold = 0.1f;
    [SerializeField] private float bobSideFrequencyMultiplier = 0.75f;
    [SerializeField] private float returnSpeed = 5f;
    private Vector3 camInitialPos;
    private float bobTimer = 0f;

    private Rigidbody playerRigidBody;

    private GameObject pointedObject = null;
    private IHoverable currentHovered = null;

    

    protected override void Awake()
    {
        base.Awake();

        this.playerRigidBody = this.GetComponent<Rigidbody>();

        this.playerRigidBody.useGravity = false;
        this.playerRigidBody.isKinematic = false;
        this.playerRigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        this.playerRigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        this.playerRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        this.onInteract += this.Interact;

        this.camInitialPos = this.cameraTransform.localPosition;
    }

    private void Update()
    {
        this.HandleRotation();
        this.DetectObjectUnderCursor();
        this.UpdateHover();
        this.HandleCameraBob();
    }

    private void FixedUpdate()
    {
        this.HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 forward = this.transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = this.transform.right; right.y = 0f; right.Normalize();

        if (this.moveInput.x != 0f || this.moveInput.y != 0f)
        {
            Vector3 moveDir = forward * this.moveInput.y + right * this.moveInput.x;
            Vector3 move = this.moveSpeed * Time.fixedDeltaTime * moveDir.normalized;

            if (!this.playerRigidBody.SweepTest(moveDir.normalized, out RaycastHit hit, move.magnitude))
            {
                this.playerRigidBody.linearVelocity = moveDir.normalized * this.moveSpeed;

                return;
            }
        }

        this.playerRigidBody.linearVelocity = Vector3.zero;
        this.playerRigidBody.angularVelocity = Vector3.zero;
    }

    private void HandleRotation()
    {
        if (this.rotateInput != 0f)
        {
            float rotateY = this.rotateInput * this.rotationSpeed * Time.deltaTime;
            this.transform.Rotate(0f, rotateY, 0f, Space.World);
        }
    }

    private void DetectObjectUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            this.pointedObject = hit.collider.gameObject;
        }    
        else
        {
            this.pointedObject = null;
        }   
    }

    private void UpdateHover()
    {
        if (    this.pointedObject == null 
            || !this.pointedObject.TryGetComponent<IHoverable>(out IHoverable hoverable))
        {
            if (this.currentHovered != null)
            {
                this.currentHovered.OnHoverExit();
                this.currentHovered = null;
            }
        }
        else if (this.currentHovered == null)
        {
            this.currentHovered = hoverable;
            this.currentHovered.OnHover();
        }
        else if (this.currentHovered != hoverable)
        {
            this.currentHovered.OnHoverExit();
            this.currentHovered = hoverable;
            this.currentHovered.OnHover();
        }
    }

    private void Interact()
    {
        if (   this.pointedObject != null 
            && this.pointedObject.GetComponentInParent<IInteractive>() is IInteractive interactable)  
        {
            if( Vector3.Distance(this.transform.position, this.pointedObject.transform.position) <= this.interactRange)
            {
                interactable.Interact();
            }
            else
            {
                FindFirstObjectByType<UIManager>().ShowMessage(this.tooFarMessage);
            }
        }
    }


    private void HandleCameraBob()
    {
        float speed = this.playerRigidBody.linearVelocity.magnitude;

        if (speed > this.speedThreshold)
        {
            this.bobTimer += Time.deltaTime * this.verticalBobFreq;

            float yOffset = Mathf.Sin(this.bobTimer) * this.bobHeight;
            float xOffset = Mathf.Sin(this.bobTimer * this.bobSideFrequencyMultiplier)
                * this.bobHeight;

            this.cameraTransform.localPosition =
                this.camInitialPos + new Vector3(xOffset, yOffset, 0);
        }
        else
        {
            this.bobTimer = 0f;
            this.cameraTransform.localPosition = Vector3.Lerp(
                this.cameraTransform.localPosition,
                this.camInitialPos,
                Time.deltaTime * this.returnSpeed
            );
        }
    }


    public void SetTransform(Transform transform)
    {
        this.playerRigidBody.position = transform.position;
        this.playerRigidBody.rotation = transform.rotation;

        this.StopMoving();
    }

    public void StopMoving()
    {
        this.moveInput = Vector2.zero;
        this.rotateInput = 0f;

        this.playerRigidBody.linearVelocity = Vector3.zero;
        this.playerRigidBody.angularVelocity = Vector3.zero;
    }
}
