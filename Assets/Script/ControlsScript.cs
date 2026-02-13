using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ControlsScript : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference MoveActionRef;
    [SerializeField] private InputActionReference LookActionRef;
    [SerializeField] private InputActionReference PhotoActionRef;

    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float smoothTime = 0.05f;

    private CharacterController controller;
    private Vector3 velocity;

    private float xRotation = 0f;
    private Vector2 currentLook;
    private Vector2 lookVelocity;

    void OnEnable()
    {
        MoveActionRef.action.Enable();
        LookActionRef.action.Enable();
        PhotoActionRef.action.Enable();

        PhotoActionRef.action.performed += takePhoto;
    }

    void OnDisable()
    {
        PhotoActionRef.action.performed -= takePhoto;

        MoveActionRef.action.Disable();
        LookActionRef.action.Disable();
        PhotoActionRef.action.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Look();
    }

    void takePhoto(InputAction.CallbackContext ctx)
    {
        PhotoCapture photoComp = GetComponent<PhotoCapture>();
        if (!photoComp.viewingPhoto)
        {
            StartCoroutine(photoComp.CapturePhoto());
        }
        else
        {
            photoComp.RemovePhoto();
        }
    }

    void Move()
    {
        Vector2 input = MoveActionRef.action.ReadValue<Vector2>();

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        Vector2 targetLook = LookActionRef.action.ReadValue<Vector2>() * mouseSensitivity;

        currentLook = Vector2.SmoothDamp(
            currentLook,
            targetLook,
            ref lookVelocity,
            smoothTime
        );

        xRotation -= currentLook.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * currentLook.x);
    }
}
