using UnityEngine;
using UnityEngine.UI;
using System.Collections;


struct Directions
{
    public float ToForward;
    public float ToRight;
}

public class PlayerQ3LikeController : MonoBehaviour
{
    // Camera
    public Transform firstPersonView;
    public Transform thirdPersonView;
    private Transform _currentView;
    [SerializeField]private float playerViewYOffset = 0.6f; // The height at which the camera is bound to

    public float xMouseSensitivity;
    public float yMouseSensitivity;

    public float gravity;
    public bool useCustomGravity;
    public float customGravity;
    private bool _previousUseCustomGravity;
    [SerializeField] private float defaultGravity = 20f;
    [SerializeField]private float groundFriction = 6;

    [SerializeField]private float speedOnGround = 7.0f;
    [SerializeField]private float runAcceleration = 14.0f;
    [SerializeField]private float runDeacceleration = 10.0f;
    [SerializeField]private float airAcceleration = 2.0f;
    [SerializeField]private float airDecceleration = 2.0f;           //Deacceleration experienced when oposite strafing
    [SerializeField]private float airControl = 0.3f;                //How precise air control is
    [SerializeField]private float sideStrafeAcceleration = 50.0f;  //How fast acceleration occurs to get up to sideStrafeSpeed when
    [SerializeField]private float sideStrafeSpeed = 1.0f;         //What the max speed to generate when side strafing
    [SerializeField]private float jumpSpeed = 8.0f;              //The speed at which the character's up axis gains when hitting jump
    [SerializeField]private bool holdJumpToBhop;                //Enables bhop when the jump button is pressed

    // Q3: players can queue the next jump just before he hits the ground
    private bool _wishJump; //false by default

    //Running
    private bool _isRunning;
    public float mana = 5f;
    [SerializeField]private float maxMana = 6f;
    [SerializeField]private float manaDrainRate = 0.9f;
    [SerializeField]private float manaRecoveryRate = 0.8f;
    [SerializeField]private float runMultiplier = 2f;

    public Armor armor;
    private Console _console;
    public GUIStyle style;
    private CharacterController _controller;

    //Wall handler
    public LayerMask whatIsWall;
    private bool _isWallRight, _isWallLeft,_isWallBack,_isWallFront;
    private bool _isWallRunning;
    public float maxWallRunCameraTilt;
    private float _jumpReleaseTime,_jumpPressTime; 
    private float maxWallSpeed = 35f;
    
    //FPS
    public float fpsDisplayRate = 4.0f; // 4 updates per sec
    private int _frameCount; //0 by default
    private float _dt;      //0.0f by default
    private float _fps;    //0.0f by default

    // Camera rotations
    private float _rotX; //0.0f by default

    private float _rotY; //0.0f by default
    // Player commands, stores wish commands that the player asks for (Forward, Right)
    private Directions _dirs;
    private Vector3 _moveDirectionNorm = Vector3.zero;
    public Vector3 playerVelocity = Vector3.zero;
    private float _playerTopVelocity;//0.0f by default

    // Used to display real time fricton values
    private float _playerFriction;//0.0f by default
    
    //Сoefficients
    //private float _jumpPowerCoeff = 2f;
    private float _wallJumpCostCoeff = 2.5f;
    private float _wallRunCostCoeff = 3f;
    private float _wallRunVelocityMultiplier = 1.03f;
    private float _windJumpCostCoeff = 2f;
    
    // Wall contact 
    private Vector3 _wallContactNormal;
    
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    //public float sphereRadius = 0.5f; 
    public LayerMask groundMask;
    private bool isGrounded;

    public Image Crosshair;
     
    private float _raycastDistance = 1f;
    private bool _isWallJumping;

    //Spells
    public bool windSpellInUse;
    public bool fireSpellInUse;

    
    
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

       Crosshair = GameObject.FindObjectOfType<Image>();

       Crosshair.gameObject.SetActive(true);
    }

    
    private void HandleInput()
    {
        _rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        _rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        _rotX = Mathf.Clamp(_rotX, -90, 90);

        transform.rotation = Quaternion.Euler(0, _rotY, 0);
        _currentView.rotation = Quaternion.Euler(_rotX, _rotY, 0);
    }
    
    private void Update()
    {
        if (!PauseMenuSingleton.Instance.IsPaused)
        {
            xMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            yMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
	        Crosshair.gameObject.SetActive(true);
        }
        else
        {
	        Crosshair.gameObject.SetActive(false);
		    return;
        }
        
        
        
        //RaycastHit hit;
        // Vector3 sphereCastOrigin = groundCheck.position + Vector3.up * sphereRadius;
        //Vector3 direction = -Vector3.up;
        //isGrounded = Physics.SphereCast(sphereCastOrigin, sphereRadius, direction, out hit, groundDistance + sphereRadius, groundMask);
        
        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance, groundMask);
        
        HandleInput();
        
        //Spells 
        if (Input.GetKeyDown(KeyCode.Alpha1)) windSpellInUse = !windSpellInUse;  //Button 1
        if (Input.GetKeyDown(KeyCode.Alpha2)) fireSpellInUse = !fireSpellInUse;  //Button 2
        
        
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
            if (Input.GetButtonDown("Fire1")) Cursor.lockState = CursorLockMode.Locked;
        
        if(!windSpellInUse)QueueJump();

        if(isGrounded) GroundMove();
        else if(!isGrounded) AirMove();

        // Move the controller
        _controller.Move(playerVelocity * Time.deltaTime);

        //Top velocity calc
        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if(udp.magnitude > _playerTopVelocity) _playerTopVelocity = udp.magnitude;

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

        mana = Mathf.Clamp(mana, 0, maxMana);

        if (isGrounded && Input.GetKey(KeyCode.LeftShift) && windSpellInUse)
        {
            float manaCost = manaDrainRate * Time.deltaTime * (1 + armor.weight / 20);
            if (mana >= manaCost)
            {
                _isRunning = true;
                mana -= manaCost;
            }
            else
            {
                _isRunning = false;
                Debug.Log("Not enough mana for running");
            }
        }
        else
        {
            _isRunning = false;
            if (mana < maxMana)
            {
                float recoveryMultiplier = 1 / (1 + armor.weight / 20);
                mana += manaRecoveryRate * recoveryMultiplier * Time.deltaTime;
            }
        }
        
        if (windSpellInUse)
        {
            // CheckForWall();
            CheckForWall();
            WallRunInput();
            WindSpellJump();
        }
        
        
        if (_isWallRunning && windSpellInUse && !isGrounded)
	    {
	        if (_isWallRight || _isWallLeft)
	        {
		        float tiltDirection = _isWallRight ? 1 : -1;
		        float targetTilt = maxWallRunCameraTilt * tiltDirection;
		        float targetVerticalRotation = _currentView.localRotation.eulerAngles.x;

		        Quaternion targetRotation = Quaternion.Euler(targetVerticalRotation, 0, targetTilt);
		        _currentView.localRotation = Quaternion.Lerp(_currentView.localRotation, targetRotation, 225f);
	        }
            else gravity = 0f;
	        if (windSpellInUse) WallJump();
	    }
	    else
	    {
	        if (useCustomGravity) gravity = customGravity;
	        else gravity = defaultGravity;
	    }
        
       if (_isWallJumping) _raycastDistance = 0f;
        
        else if (_isWallRunning && windSpellInUse && !isGrounded)
        {
            playerVelocity.y = 0f;
            _raycastDistance = 3f;
        }
        else _raycastDistance = 1f;
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

        if(Input.GetButtonDown("Jump") && !_wishJump) _wishJump = true;
        if(Input.GetButtonUp("Jump")) _wishJump = false;
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

        if (Vector3.Dot(playerVelocity, wishdir) < 0) accel = airDecceleration;
        else accel = airAcceleration;

        // If the player is ONLY strafing left or right
        if(_dirs.ToForward == 0 && _dirs.ToRight != 0)
        {
            if(wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;

            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);

        if(airControl > 0) AirControl(wishdir, wishspeed2);

        // Apply gravity
        playerVelocity.y -= gravity * Time.deltaTime;
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
        if(Mathf.Abs(_dirs.ToForward) < 0.001 || Mathf.Abs(wishspeed) < 0.001) return;

        zSpeed = playerVelocity.y;
        playerVelocity.y = 0;
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dotProd = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= airControl * dotProd * dotProd * Time.deltaTime;

        // Change direction while slowing down
        if (dotProd > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;
            playerVelocity.Normalize();
            _moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zSpeed;
        playerVelocity.z *= speed;
    }


    //Called every frame when the engine detects that the player is on the ground
    private void GroundMove()
    {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if (!_wishJump) ApplyFriction(1.0f);
        else ApplyFriction(0);

        SetMovementDir();

        wishdir = new Vector3(_dirs.ToRight, 0, _dirs.ToForward);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        _moveDirectionNorm = wishdir;
        float wishspeed = wishdir.magnitude;

        if (_isRunning) wishspeed *= runMultiplier * speedOnGround;
        else wishspeed *= speedOnGround;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        playerVelocity.y = -gravity * Time.deltaTime;

        if (!windSpellInUse)
        {
            if (_wishJump)
            {
                playerVelocity.y = jumpSpeed;
                _wishJump = false;
            }
        }
        else WindSpellJump();
        
        //Zeroing the velocity vector if the player does not press keys to move
        if (_dirs.ToForward == 0 && _dirs.ToRight == 0)  
        {
            playerVelocity.x = 0f;
            playerVelocity.z = 0f;
        }
    }


     //Applies friction to the player everywhere
    private void ApplyFriction(float t)
    {
        /*Any changes made to vec after this assignment
         will not affect the original _playerVelocity vector,
         since vec is a separate copy of vec*/

        Vector3 vec = playerVelocity;
        float speed;
        float newSpeed;
        float control;
        float drop;
       
        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        //Only if the player is on the ground
        if(isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * groundFriction * Time.deltaTime * t;
        }

        newSpeed = speed - drop;
        _playerFriction = newSpeed;

        if(newSpeed < 0) newSpeed = 0;
        if(speed > 0) newSpeed /= speed;

        playerVelocity.x *= newSpeed;
        playerVelocity.z *= newSpeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addSpeed;
        float accelSpeed;
        float currentSpeed;

        currentSpeed = Vector3.Dot(playerVelocity, wishdir);
        addSpeed = wishspeed - currentSpeed;

        if(addSpeed <= 0) return;

        accelSpeed = accel * Time.deltaTime * wishspeed;

        if(accelSpeed > addSpeed) accelSpeed = addSpeed;

        playerVelocity.x += accelSpeed * wishdir.x;
        playerVelocity.z += accelSpeed * wishdir.z;
    }


    private void WallRunInput() //make sure to call in Update()
    {
        if (_isWallRight) StartWallRun();
        if (_isWallLeft) StartWallRun();
        if(_isWallBack) StartWallRun();
        if(_isWallFront) StartWallRun();
        
    }

    private void StartWallRun()
    {
        float manaCost = manaDrainRate * _wallRunCostCoeff * Time.deltaTime * (1 + armor.weight / 20);
        
        
        if (mana >= manaCost && windSpellInUse)
        {
            playerVelocity.y = 0;
            _isWallRunning = true;
            
            //raycastDistance = 1.5f;    
            
            // Check if the player is moving forward or sideways
            bool movingForward = Mathf.Abs(_dirs.ToForward) > 0.1f;
            bool movingSideways = Mathf.Abs(_dirs.ToRight) > 0.1f;

            if (movingForward || movingSideways) // Check if player is moving on the wall
            {
                // WALL SUPER RUN EPTA
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    playerVelocity.x *= _wallRunVelocityMultiplier;
                    // Clamp the horizontal velocity to the maximum run speed
                    playerVelocity.x = Mathf.Clamp(playerVelocity.x, -maxWallSpeed, maxWallSpeed);
                    mana -= manaCost;
                }
                else playerVelocity.x = Mathf.Clamp(playerVelocity.x * 1.5f, -speedOnGround, speedOnGround);
            }
            else playerVelocity = Vector3.zero;
        }
        else
        {
            _isWallRunning = false;
            playerVelocity.y = -gravity;
        }

        if(mana < manaCost) Debug.Log("Not enough mana to wall run");
    }

    
    private void WallJump()
    {
        if (!_isWallRunning) return;

        if (Input.GetButtonDown("Jump")) _jumpPressTime = Time.time;
    
        if (Input.GetButtonUp("Jump"))
        {
            _jumpReleaseTime = Time.time;
            
            // Zeroing out the stored velocity momentum in a collision with a wall
            playerVelocity = Vector3.zero;

            
            _isWallJumping = true;

            StartCoroutine(ResetWallJumpFlag());
            
            
            // Calculate jump force based on how long the jump button was held
            float jumpPressDuration = _jumpReleaseTime - _jumpPressTime;
            float maxJumpPressDuration = 1.0f; // Maximum duration for full jump force
            float jumpForce = Mathf.Lerp(35f, 75f, jumpPressDuration / maxJumpPressDuration); // Linearly interpolate jump force based on press duration
            float maxJumpDuration = 0.5f;

            float jumpDuration = _jumpReleaseTime - _jumpPressTime;
            jumpDuration = Mathf.Clamp(jumpDuration, 0f, maxJumpDuration);

            // Get the direction from player to the point they're looking at
            Vector3 lookAtPoint = transform.position + Camera.main.transform.forward;
            Vector3 jumpDirection = (lookAtPoint - transform.position).normalized;

            // Calculate jump height based on jump duration
            float jumpHeight = (jumpDuration * jumpForce) / 2.5f;

            float maxJumpHeight = 5f; 
            jumpHeight = Mathf.Clamp(jumpHeight, 0f, maxJumpHeight);

            // Calculate mana cost based on jump distance relative to max jump distance
            float maxJumpDistance = 10f;
            float jumpDistance = Mathf.Min(jumpHeight, maxJumpDistance);
            float manaCost = jumpDistance / maxJumpDistance * _wallJumpCostCoeff;

        
            if (mana >= manaCost)
            {
                Vector3 jumpVelocity = jumpDirection * jumpForce + Vector3.up * jumpHeight;
                playerVelocity += jumpVelocity;
                _controller.Move(playerVelocity * Time.deltaTime);
                mana -= manaCost;
                StartCoroutine(DicreaseJump());
            }
            else Debug.Log("Not enough mana for wall jump");
        }
    }
    


    private IEnumerator DicreaseJump()
    {
        yield return new WaitForSeconds(0.8f);
        playerVelocity -= Vector3.up * (playerVelocity.y * 2f); 
    }
    
    
    private IEnumerator ResetWallJumpFlag()
    {
        yield return new WaitForSeconds(0.1f); 
        _isWallJumping = false;
    }
    
    

    private void WindSpellJump()
    {
        if (_isWallRunning) return;

        if (Input.GetButtonDown("Jump")) _jumpPressTime = Time.time;

        if (Input.GetButtonUp("Jump"))
        {
            _jumpReleaseTime = Time.time;

            float maxJumpDistance = 20f;
            float minJumpDuration = 0.5f;
            float jumpDuration = _jumpReleaseTime - _jumpPressTime;
            float jumpDistance = Mathf.Min(jumpDuration * maxJumpDistance / minJumpDuration, maxJumpDistance);

            // Calculate mana consumption based on actual jump duration
            float manaCost = jumpDistance / maxJumpDistance *_windJumpCostCoeff;
            //Debug.Log(manaCost);
            if (mana >= manaCost)
            {
                playerVelocity.y = jumpDistance;
                mana -= manaCost;
            }else Debug.Log("Not enough mana for wind jump");
        }
    }

   
    private void CheckForWall()
    {
        Vector3 playerPosition = transform.position;

        _isWallRight = Physics.Raycast(playerPosition, transform.right, _raycastDistance, whatIsWall);
        _isWallLeft = Physics.Raycast(playerPosition, -transform.right, _raycastDistance, whatIsWall);
        _isWallBack = Physics.Raycast(playerPosition, -transform.forward, _raycastDistance, whatIsWall);
        _isWallFront = Physics.Raycast(playerPosition, transform.forward, _raycastDistance, whatIsWall);

        if (!_isWallLeft && !_isWallRight && !_isWallBack && !_isWallFront) _isWallRunning = false;
        else if (_isWallJumping) playerVelocity = Vector3.zero;
    } 


    private void OnGUI()
    {
        var ups = _controller.velocity;
        ups.y = 0;
        string windSpell = windSpellInUse ? "Wind Spell in use" : "Wind Spell not in use";
        string ground = isGrounded ? "Grounded" : "Not Grounded";
        string fireSpell = fireSpellInUse ? "Fire Spell in use" : "Fire Spell not in use";

        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + _fps, style);
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100, style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(_playerTopVelocity * 100) / 100 , style);
        GUI.Label(new Rect(0, 45, 400, 100), "Mana: " + mana , style);
        GUI.Label(new Rect(0, 60, 400, 100), windSpell, style);
        GUI.Label(new Rect(0, 75, 400, 100), fireSpell, style);
        GUI.Label(new Rect(0, 90, 400, 100), ground, style);
    }
}
