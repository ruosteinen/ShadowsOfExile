using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Using [SerializeField] to make a private field available in editor
    
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private float rotationSpeed = 75f;
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float runningSpeed = 70f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 6f;

    private CharacterController _controller;
    private Vector2 _rotation;
    private Vector3 _velocity;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (firstPersonCamera == null)
            firstPersonCamera = GetComponentInChildren<Camera>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Movement();
        Rotation();
        
        //Apply gravity
        if (!_controller.isGrounded)
            _velocity.y += gravity * Time.deltaTime; 
        
        
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Movement()
    {
        if (_controller.isGrounded)
        {
            Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            Vector3 moveDirection = Quaternion.Euler(0, firstPersonCamera.transform.eulerAngles.y, 0) * new Vector3(inputDirection.x, 0, inputDirection.y);
            float deltaTimeSpeed = (Input.GetKey(KeyCode.LeftShift) ? runningSpeed : walkingSpeed) * Time.deltaTime;
            Vector3 movement = moveDirection * deltaTimeSpeed; 
            _controller.Move(movement);
            
            //JUMP!
            if (Input.GetKeyDown(KeyCode.Space))
                _velocity.y = jumpForce;
        }
        else
        {
            //TODO Make the player move by inertia from the previous movement before jumping
             _velocity.x = 0;
             _velocity.z = 0;
        }
    }

    private void Rotation()
    {
        float rotationMultiplier = rotationSpeed * Time.deltaTime;
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * rotationMultiplier;

        _rotation.y += mouseInput.x;
        _rotation.x -= mouseInput.y;
        _rotation.x = Mathf.Clamp(_rotation.x, -90f, 90f);

        firstPersonCamera.transform.localEulerAngles = new Vector3(_rotation.x, _rotation.y, 0f);
        transform.eulerAngles = new Vector3(0, _rotation.y, 0);
    }
}