using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private float rotationSpeed = 75f;
    [SerializeField] private float walkingSpeed = 6f;
    [SerializeField] private float runningSpeed = 12f;
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
        
        if (!_controller.isGrounded)
            _velocity.y += gravity * Time.deltaTime;
        
        else if (_velocity.y < 0)
            _velocity.y = -2f; //player grounding
        

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Movement()
    {
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = inputDirection.magnitude > 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && inputDirection.y > 0;
        float currentSpeed = isRunning ? runningSpeed : walkingSpeed;

        if (_controller.isGrounded)
        {
            if (isMoving)
            {
                Vector3 moveDirection = Quaternion.Euler(0, firstPersonCamera.transform.eulerAngles.y, 0) 
                                        * new Vector3(inputDirection.x, 0, inputDirection.y).normalized;
                
                Vector3 movement = moveDirection * currentSpeed;
                _velocity.x = movement.x;
                _velocity.z = movement.z;
            }
            else
            {
                //Cancellation of horizontal movement during a jump from a standing position
                _velocity.x = 0;
                _velocity.z = 0;
            }

           
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _velocity.y = jumpForce;
    
                if (isMoving)
                {
                    float jumpModifier = isRunning ? 0.5f : 0.3f;
                    float addedSpeed = currentSpeed * jumpModifier;
                    _velocity += firstPersonCamera.transform.forward * addedSpeed;
                }
            }
        }
        else
        {
            //gravity action while the player is in the air
            _velocity.y += gravity * Time.deltaTime;
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
