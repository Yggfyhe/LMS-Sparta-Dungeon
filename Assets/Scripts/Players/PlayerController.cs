using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 _curMovementInput;
    public Vector2 CurMovementInput { get { return _curMovementInput; } }
    public float jumpForce;
    public LayerMask groundLayerMask;

    public bool IsPlayerGrounded { get { return IsGrounded(); } }

    public event Action<Vector2> OnMoveEvent;
    public event Action OnJumpEvent;

    [Header("Look")]
    public Transform cameraContainer;
    public float lookSensitivity;
    public float distanceFromPlayer = 5.0f;
    public float verticalOffset = 1.5f;
    public float cameraSmoothSpeed = 10.0f;

    private Vector2 _mouseDelta;
    private float _camYaw;
    private float _camPitch;
    public float minXLook = -20f;
    public float maxXLook = 60f;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Move();
        if (canLook)
        {
            CameraLook();
        }
    }

    private void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        // 점프 중일 때 x와 z 축 회전을 고정
        if (!IsGrounded())
        {
            Quaternion currentRotation = transform.rotation;
            currentRotation.x = 0;
            currentRotation.z = 0;
            transform.rotation = currentRotation;
        }
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(_curMovementInput);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
            OnMoveEvent?.Invoke(_curMovementInput);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            OnJumpEvent?.Invoke(); 
            StartCoroutine(JumpWithDelay(1.0f)); 
        }
    }

    private IEnumerator JumpWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
    }

    private void Move()
    {
        if (_curMovementInput.sqrMagnitude > 0.01f)
        {
            Vector3 forward = cameraContainer.forward;
            Vector3 right = cameraContainer.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * _curMovementInput.y + right * _curMovementInput.x;
            desiredMoveDirection *= moveSpeed;

            desiredMoveDirection.y = _rigidbody.velocity.y;

            _rigidbody.velocity = desiredMoveDirection;

            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            if (IsGrounded())
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            }
        }
    }

    private void CameraLook()
    {
        _camYaw += _mouseDelta.x * lookSensitivity;
        _camPitch -= _mouseDelta.y * lookSensitivity;
        _camPitch = Mathf.Clamp(_camPitch, minXLook, maxXLook);

        Quaternion rotation = Quaternion.Euler(_camPitch, _camYaw, 0);
        Vector3 desiredPosition = transform.position - (rotation * Vector3.forward * distanceFromPlayer) + (Vector3.up * verticalOffset);

        cameraContainer.position = Vector3.Lerp(cameraContainer.position, desiredPosition, Time.deltaTime * cameraSmoothSpeed);
        cameraContainer.LookAt(transform.position + Vector3.up * verticalOffset);
    }

    bool IsGrounded()
    {
        float rayLength = 0.1f;
        Vector3 origin = transform.position + (transform.up * 0.01f);

        return Physics.Raycast(origin, Vector3.down, rayLength, groundLayerMask);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
