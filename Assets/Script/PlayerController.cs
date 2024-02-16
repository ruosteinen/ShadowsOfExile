using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rotationSpeed = 75f;
    [SerializeField] private float walkingSpeed = 6f;
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 6f;

    private CharacterController _controller;
    private Vector2 _rotation;
    private Vector3 _velocity;
    private float _currentRunSpeed;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _currentRunSpeed = 0f;

        
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    public void ToggleCursorLock(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    
    
    private void Update()
    {
        Movement();
        Rotation();

        if (!_controller.isGrounded)
            _velocity.y += gravity * Time.deltaTime;
        
        else if (_velocity.y < 0)
            _velocity.y = -2f;
        
        
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Movement()
    {
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = inputDirection.magnitude > 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && inputDirection.y > 0;

        if (_controller.isGrounded)
        {
            _velocity.y = -2f;

            if (isMoving)
            {
                if (isRunning)
                {
                    // Smoothly increase speed up to the maximum
                    _currentRunSpeed += acceleration * Time.deltaTime;
                    _currentRunSpeed = Mathf.Min(_currentRunSpeed, runningSpeed);
                }
                else
                {
                    // Use walking speed if not running
                    _currentRunSpeed = walkingSpeed;
                }

                Vector3 moveDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * 
                                        new Vector3(inputDirection.x, 0, inputDirection.y).normalized;
                Vector3 movement = moveDirection * _currentRunSpeed;
                _velocity.x = movement.x;
                _velocity.z = movement.z;
            }
            else
            {
                _velocity.x = 0;
                _velocity.z = 0;
                if (!isRunning) // Reset running speed if not moving and not holding run
                    _currentRunSpeed = 0;
                
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
                _velocity.y = jumpForce; // Apply jump force
            
        }
        
        _velocity.y += gravity * Time.deltaTime;
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        _rotation.y += mouseX;
        _rotation.x -= mouseY;
        _rotation.x = Mathf.Clamp(_rotation.x, -90f, 90f);
        
        playerCamera.transform.localEulerAngles = new Vector3(_rotation.x, _rotation.y, 0f);
        transform.localEulerAngles = new Vector3(0f, _rotation.y, 0f);
    }
}