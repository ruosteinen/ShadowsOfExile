using UnityEngine;

// Contains the command the user wishes upon the character
struct Directions
{
    public float ToForward;
    public float ToRight;
}

public class PlayerQ3LikeController : MonoBehaviour
{
    public Transform firstPersonView;// Camera
    public Transform thirdPersonView; 
    private Transform _currentView;
    
    
    [SerializeField]private float playerViewYOffset = 0.6f; // The height at which the camera is bound to
    public float xMouseSensitivity;
    public float yMouseSensitivity;

    
    public float gravity = 20.0f;
    [SerializeField]private float groundFriction = 6;
    [SerializeField]private float speedOnGround = 7.0f;
    [SerializeField]private float runAcceleration = 14.0f;
    [SerializeField]private float runDeacceleration = 10.0f;
    [SerializeField]private float airAcceleration = 2.0f;
    [SerializeField]private float airDecceleration = 2.0f;          //Deacceleration experienced when oposite strafing
    [SerializeField]private float airControl = 0.3f;               //How precise air control is
    [SerializeField]private float sideStrafeAcceleration = 50.0f;  //How fast acceleration occurs to get up to sideStrafeSpeed when
    [SerializeField]private float sideStrafeSpeed = 1.0f;         //What the max speed to generate when side strafing
    [SerializeField]private float jumpSpeed = 8.0f;              //The speed at which the character's up axis gains when hitting jump
    [SerializeField]private bool holdJumpToBhop;                 //Enables bhop when the jump button is pressed
    
    
    //Running
    [SerializeField]private float stamina = 5f;
    [SerializeField]private float maxStamina = 6f;
    [SerializeField]private float staminaDrainRate = 0.9f;
    [SerializeField]private float staminaRecoveryRate = 0.8f;
    [SerializeField]private float runMultiplier = 2f;
    public Armor armor;
    private bool _isRunning; 
    
    
    
    
    
    
    public GUIStyle style;

    //FPS
    public float fpsDisplayRate = 4.0f; // 4 updates per sec

    private int _frameCount; //0 by default
    private float _dt;      //0.0f by default
    private float _fps;    //0.0f by default

    private CharacterController _controller;

    // Camera rotations
    private float _rotX; //0.0f by default
    private float _rotY; //0.0f by default

    private Vector3 _moveDirectionNorm = Vector3.zero;
    private Vector3 _playerVelocity = Vector3.zero;
    private float _playerTopVelocity;//0.0f by default

    // Q3: players can queue the next jump just before he hits the ground
    private bool _wishJump; //false by default

    // Used to display real time fricton values
    private float _playerFriction;//0.0f by default

    // Player commands, stores wish commands that the player asks for (Forward, Right)
    private Directions _dirs;
    
    private void Start()
    {
        
        _currentView = firstPersonView;
        firstPersonView.gameObject.SetActive(true);
        thirdPersonView.gameObject.SetActive(false);
        
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonView  == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                firstPersonView = mainCamera.gameObject.transform;
        }

        // Put the camera inside the capsule collider
        Vector3 currentPosition = transform.position;
        _currentView.position = new Vector3(currentPosition.x, currentPosition.y + playerViewYOffset, currentPosition.z);
        _controller = GetComponent<CharacterController>();
    
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        xMouseSensitivity = sensitivity;
        yMouseSensitivity = sensitivity;
        
        armor = GetComponent<Armor>();
        
    }
    
    private void Update()
    {
        if (PauseMenuSingleton.Instance.IsPaused)
        {
            return;
        }
        
        if (!PauseMenuSingleton.Instance.IsPaused)
        {
            xMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            yMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        
        //FPS calculation
        _frameCount++;
        _dt += Time.deltaTime;
        if (_dt > 1.0 / fpsDisplayRate)
        {
            _fps = Mathf.Round(_frameCount / _dt);
            _frameCount = 0;
            _dt -= 1.0f / fpsDisplayRate;
        }
        
        //Cursor locking
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }
        
        _rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        _rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        // Clamp the X rotation
        /*if(_rotX < -90)
            _rotX = -90;
        else if(_rotX > 90)
            _rotX = 90;*/

        _rotX = Mathf.Clamp(_rotX, -90, 90);
        
        
        transform.rotation = Quaternion.Euler(0, _rotY, 0); // Rotates body
        _currentView.rotation = Quaternion.Euler(_rotX, _rotY, 0); // Rotates camera

        
        QueueJump();
        if(_controller.isGrounded)
            GroundMove();
        else if(!_controller.isGrounded)
            AirMove();

        // Move the controller
        _controller.Move(_playerVelocity * Time.deltaTime);

        //Top velocity calc
        Vector3 udp = _playerVelocity;
        udp.y = 0.0f;
        if(udp.magnitude > _playerTopVelocity)
            _playerTopVelocity = udp.magnitude;
        
        
        Vector3 currentPosition = transform.position;
        _currentView.position = new Vector3(currentPosition.x, 
            currentPosition.y + playerViewYOffset,
            currentPosition.z);
        
        
        if (Input.GetKeyDown(KeyCode.C)) {
            if (_currentView == firstPersonView) {
                firstPersonView.gameObject.SetActive(false);
                thirdPersonView.gameObject.SetActive(true);
                _currentView = thirdPersonView;
            } else {
                thirdPersonView.gameObject.SetActive(false);
                firstPersonView.gameObject.SetActive(true);
                _currentView = firstPersonView;
            }
        }
        
        
        
        if (_controller.isGrounded && stamina > 1 && Input.GetKey(KeyCode.LeftShift))
        {
            _isRunning = true;
            stamina -= staminaDrainRate * Time.deltaTime * (1 + armor.weight / 20);
            Debug.Log(armor.weight);
            Debug.Log(stamina);
        }
        else
        {
            _isRunning = false;
            if (stamina < maxStamina)
            {
                float recoveryMultiplier = 1 / (1 + armor.weight / 20);
                stamina += staminaRecoveryRate * recoveryMultiplier * Time.deltaTime;
            }
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        
        HandleInput();
    }
    
    
    private void HandleInput()
    {
        _rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        _rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        _rotX = Mathf.Clamp(_rotX, -90, 90);

        transform.rotation = Quaternion.Euler(0, _rotY, 0);
        _currentView.rotation = Quaternion.Euler(_rotX, _rotY, 0);
    }
    
    
    private void SetMovementDir()
    {
        _dirs.ToForward = Input.GetAxisRaw("Vertical");
        _dirs.ToRight = Input.GetAxisRaw("Horizontal");
    }
    
    
     //Queues the next jump just like in Q3
     private void QueueJump()
    {
        if(holdJumpToBhop)
        {
            _wishJump = Input.GetButton("Jump");
            return;
        }

        if(Input.GetButtonDown("Jump") && !_wishJump)
            _wishJump = true;
        if(Input.GetButtonUp("Jump"))
            _wishJump = false;
    }

   //Execs when the player is in the air
    private void AirMove()
    {
        Vector3 wishdir;
        //float wishvel = airAcceleration;
        float accel;
        
        SetMovementDir();

        wishdir =  new Vector3(_dirs.ToRight, 0, _dirs.ToForward);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= speedOnGround;

        wishdir.Normalize();
        _moveDirectionNorm = wishdir;
        
        
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(_playerVelocity, wishdir) < 0)
            accel = airDecceleration;
        else
            accel = airAcceleration;
        
        
        // If the player is ONLY strafing left or right
        if(_dirs.ToForward == 0 && _dirs.ToRight != 0)
        {
            if(wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if(airControl > 0)
            AirControl(wishdir, wishspeed2);
       

        // Apply gravity
        _playerVelocity.y -= gravity * Time.deltaTime;
    }

    /*
     Air control occurs when the player is in the air, it allows
     players to move side to side much faster rather than being
      'sluggish' when it comes to cornering.
     */
    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zSpeed;
        float speed;
        float dotProd;
        float k;

        // Can't control movement if not moving forward or backward
        if(Mathf.Abs(_dirs.ToForward) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        
        zSpeed = _playerVelocity.y;
        _playerVelocity.y = 0;
        
        
        speed = _playerVelocity.magnitude;
        _playerVelocity.Normalize();

        dotProd = Vector3.Dot(_playerVelocity, wishdir);
        k = 32;
        k *= airControl * dotProd * dotProd * Time.deltaTime;

        // Change direction while slowing down
        if (dotProd > 0)
        {
            _playerVelocity.x = _playerVelocity.x * speed + wishdir.x * k;
            _playerVelocity.y = _playerVelocity.y * speed + wishdir.y * k;
            _playerVelocity.z = _playerVelocity.z * speed + wishdir.z * k;

            _playerVelocity.Normalize();
            _moveDirectionNorm = _playerVelocity;
        }

        _playerVelocity.x *= speed;
        _playerVelocity.y = zSpeed;
        _playerVelocity.z *= speed;
    }
    
    
    //Called every frame when the engine detects that the player is on the ground
    private void GroundMove()
    {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if (!_wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        SetMovementDir();

        wishdir = new Vector3(_dirs.ToRight, 0, _dirs.ToForward);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        
        _moveDirectionNorm = wishdir;
        
        float wishspeed = wishdir.magnitude;
        
        if (_isRunning)
            wishspeed *= runMultiplier * speedOnGround;
        else
            wishspeed *= speedOnGround;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        _playerVelocity.y = -gravity * Time.deltaTime;

        if(_wishJump)
        {
            _playerVelocity.y = jumpSpeed;
            _wishJump = false;
        }
    }

    
     //Applies friction to the player everywhere
    private void ApplyFriction(float t)
    {
        /*Any changes made to vec after this assignment
         will not affect the original _playerVelocity vector, 
         since vec is a separate copy of vec*/
        
        Vector3 vec = _playerVelocity;
        float speed;
        float newSpeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        //Only if the player is on the ground 
        if(_controller.isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * groundFriction * Time.deltaTime * t;
        }

        newSpeed = speed - drop;
        _playerFriction = newSpeed;
        
        if(newSpeed < 0)
            newSpeed = 0;
        if(speed > 0)
            newSpeed /= speed;

        
        _playerVelocity.x *= newSpeed;
        _playerVelocity.z *= newSpeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addSpeed;
        float accelSpeed;
        float currentSpeed;

        currentSpeed = Vector3.Dot(_playerVelocity, wishdir);
        addSpeed = wishspeed - currentSpeed;
        
        if(addSpeed <= 0)
            return;
        
        accelSpeed = accel * Time.deltaTime * wishspeed;
        
        if(accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        _playerVelocity.x += accelSpeed * wishdir.x;
        _playerVelocity.z += accelSpeed * wishdir.z;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + _fps, style);
        var ups = _controller.velocity;
        ups.y = 0;
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100, style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(_playerTopVelocity * 100) / 100 , style);
    }
}