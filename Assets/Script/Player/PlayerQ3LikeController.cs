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
    public Transform firstPersonView;
    private Transform _currentView;

    public float xMouseSensitivity;
    public float yMouseSensitivity;

    public float gravity;
    public bool useCustomGravity;
    public float customGravity;
    [SerializeField] private float defaultGravity = 20f;
    [SerializeField] private float groundFriction = 6;

    [SerializeField] private float speedOnGround = 7.0f;
    [SerializeField] private float runAcceleration = 14.0f;
    [SerializeField] private float runDeacceleration = 10.0f;
    [SerializeField] private float airAcceleration = 2.0f;
    [SerializeField] private float airDecceleration = 2.0f;
    [SerializeField] private float airControl = 0.3f;
    [SerializeField] private float sideStrafeAcceleration = 50.0f;
    [SerializeField] private float sideStrafeSpeed = 1.0f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private bool holdJumpToBhop;

    private bool _wishJump;
    private bool _isRunning;
    [SerializeField] private float manaDrainRate = 10f;
    [SerializeField] private float manaRecoveryRate = 8f;
    [SerializeField] private float runMultiplier = 5f;

    public Armor armor;
    private CharacterController _controller;

    public LayerMask whatIsWall;
    private bool _isWallRight, _isWallLeft, _isWallBack, _isWallFront;
    private bool _isWallRunning;
    public float maxWallRunCameraTilt;
    private float _jumpReleaseTime, _jumpPressTime;
    private float maxWallSpeed = 35f;

    public float fpsDisplayRate = 4.0f;
    private int _frameCount;
    private float _dt;
    private float _fps;

    private float _rotX;
    private float _rotY;
    private Directions _dirs;
    private Vector3 _moveDirectionNorm = Vector3.zero;
    public Vector3 playerVelocity = Vector3.zero;
    private float _playerTopVelocity;

    private float _playerFriction;

    private float _wallJumpCostCoeff = 90f;
    private float _wallRunCostCoeff = 0.8f;
    private float _wallRunVelocityMultiplier = 1.03f;
    private float _windJumpCostCoeff = 25f;

    private Vector3 _wallContactNormal;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    private bool isGrounded;

    public Image Crosshair;

    private float _raycastDistance = 1f;
    private bool _isWallJumping;

    public bool windSpellInUse;

    public Texture2D windTexture;
    private float scaleWindFactor = 1.5f;

    public PlayStats playStats;

    public GameObject townWaypoint;
    public GameObject forestWaypoint;
    public GameObject mountainWaypoint;

    public Vector3 fixedOffset = new Vector3(0, 0.7f, 1f);

    private Animator animator;

    private void Start()
    {
        _currentView = firstPersonView;
        firstPersonView.gameObject.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonView == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                firstPersonView = mainCamera.gameObject.transform;
        }

        Vector3 currentPosition = transform.position;
        _currentView.position = currentPosition + fixedOffset;

        _controller = GetComponent<CharacterController>();

        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        xMouseSensitivity = sensitivity;
        yMouseSensitivity = sensitivity;

        armor = GetComponent<Armor>();

        animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (_currentView != null)
        {
            _currentView.position = transform.position + transform.rotation * fixedOffset;
        }
    }

    private void HandleInput()
    {
        _rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        _rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        _rotX = Mathf.Clamp(_rotX, -90, 90);

        transform.rotation = Quaternion.Euler(0, _rotY, 0);
        _currentView.localRotation = Quaternion.Euler(_rotX, 0, 0);
    }

    private void Update()
    {
        
            xMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            yMouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            if (Crosshair != null) Crosshair.gameObject.SetActive(true);
        
        else
        {
            if (Crosshair != null) Crosshair.gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TeleportTo(townWaypoint);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TeleportTo(forestWaypoint);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TeleportTo(mountainWaypoint);
        }

        HandleInput();

        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance, groundMask);

        if (Input.GetKeyDown(KeyCode.Alpha1)) windSpellInUse = !windSpellInUse;

        _frameCount++;
        _dt += Time.deltaTime;
        if (_dt > 1.0 / fpsDisplayRate)
        {
            _fps = Mathf.Round(_frameCount / _dt);
            _frameCount = 0;
            _dt -= 1.0f / fpsDisplayRate;
        }

        if (!windSpellInUse) QueueJump();

        if (isGrounded) GroundMove();
        else if (!isGrounded) AirMove();

        _controller.Move(playerVelocity * Time.deltaTime);

        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if (udp.magnitude > _playerTopVelocity) _playerTopVelocity = udp.magnitude;

        Vector3 currentPosition = transform.position;
        _currentView.position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);

        playStats.currentMana = Mathf.Clamp(playStats.currentMana, 0, playStats.maxMana);

        if (isGrounded && Input.GetKey(KeyCode.LeftShift) && windSpellInUse)
        {
            float manaCost = manaDrainRate * Time.deltaTime;
            if (playStats.currentMana >= manaCost)
            {
                _isRunning = true;
                playStats.currentMana -= manaCost * 0.8f;
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
            if (playStats.currentMana < playStats.maxMana)
            {
                float recoveryMultiplier = 1 / (1 + armor.weight / 20);
                playStats.currentMana += manaRecoveryRate * recoveryMultiplier * Time.deltaTime;
            }
        }

        if (windSpellInUse)
        {
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

        UpdateAnimator();
    }

    void TeleportTo(GameObject waypoint)
    {
        if (waypoint != null)
        {
            transform.position = waypoint.transform.position;
        }
        else
        {
            Debug.Log("Invalid waypoint");
        }
    }

    private void SetMovementDir()
    {
        _dirs.ToForward = Input.GetAxisRaw("Vertical");
        _dirs.ToRight = Input.GetAxisRaw("Horizontal");
    }

    private void QueueJump()
    {
        if (holdJumpToBhop)
        {
            _wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !_wishJump) _wishJump = true;
        if (Input.GetButtonUp("Jump")) _wishJump = false;
    }

    private void AirMove()
    {
        Vector3 wishdir;
        float accel;

        SetMovementDir();
        wishdir = new Vector3(_dirs.ToRight, 0, _dirs.ToForward);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= speedOnGround;

        wishdir.Normalize();
        _moveDirectionNorm = wishdir;

        float wishspeed2 = wishspeed;

        if (Vector3.Dot(playerVelocity, wishdir) < 0) accel = airDecceleration;
        else accel = airAcceleration;

        if (_dirs.ToForward == 0 && _dirs.ToRight != 0)
        {
            if (wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;

            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);

        if (airControl > 0) AirControl(wishdir, wishspeed2);

        playerVelocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zSpeed;
        float speed;
        float dotProd;
        float k;

        if (Mathf.Abs(_dirs.ToForward) < 0.001 || Mathf.Abs(wishspeed) < 0.001) return;

        zSpeed = playerVelocity.y;
        playerVelocity.y = 0;
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dotProd = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= airControl * dotProd * dotProd * Time.deltaTime;

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

    private void GroundMove()
    {
        Vector3 wishdir;

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

        if (_dirs.ToForward == 0 && _dirs.ToRight == 0)
        {
            playerVelocity.x = 0f;
            playerVelocity.z = 0f;
        }
    }

    private void ApplyFriction(float t)
    {
        Vector3 vec = playerVelocity;
        float speed;
        float newSpeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        if (isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * groundFriction * Time.deltaTime * t;
        }

        newSpeed = speed - drop;
        _playerFriction = newSpeed;

        if (newSpeed < 0) newSpeed = 0;
        if (speed > 0) newSpeed /= speed;

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

        if (addSpeed <= 0) return;

        accelSpeed = accel * Time.deltaTime * wishspeed;

        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        playerVelocity.x += accelSpeed * wishdir.x;
        playerVelocity.z += accelSpeed * wishdir.z;
    }

    private void WallRunInput()
    {
        if (_isWallRight) StartWallRun();
        if (_isWallLeft) StartWallRun();
        if (_isWallBack) StartWallRun();
        if (_isWallFront) StartWallRun();
    }

    private void StartWallRun()
    {
        float manaCost = manaDrainRate * _wallRunCostCoeff * Time.deltaTime;

        if (playStats.currentMana >= manaCost && windSpellInUse)
        {
            playerVelocity.y = 0;
            _isWallRunning = true;

            bool movingForward = Mathf.Abs(_dirs.ToForward) > 0.1f;
            bool movingSideways = Mathf.Abs(_dirs.ToRight) > 0.1f;

            if (movingForward || movingSideways)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    playerVelocity.x *= _wallRunVelocityMultiplier;
                    playerVelocity.x = Mathf.Clamp(playerVelocity.x, -maxWallSpeed, maxWallSpeed);
                    playStats.currentMana -= manaCost;
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

        if (playStats.currentMana < manaCost) Debug.Log("Not enough mana to wall run");
    }

    private void WallJump()
    {
        if (!_isWallRunning) return;

        if (Input.GetButtonDown("Jump")) _jumpPressTime = Time.time;

        if (Input.GetButtonUp("Jump"))
        {
            _jumpReleaseTime = Time.time;

            playerVelocity = Vector3.zero;

            _isWallJumping = true;

            StartCoroutine(ResetWallJumpFlag());

            float jumpPressDuration = _jumpReleaseTime - _jumpPressTime;
            float maxJumpPressDuration = 1.0f;
            float jumpForce = Mathf.Lerp(35f, 75f, jumpPressDuration / maxJumpPressDuration);
            float maxJumpDuration = 0.5f;

            float jumpDuration = _jumpReleaseTime - _jumpPressTime;
            jumpDuration = Mathf.Clamp(jumpDuration, 0f, maxJumpDuration);

            Vector3 lookAtPoint = transform.position + Camera.main.transform.forward;
            Vector3 jumpDirection = (lookAtPoint - transform.position).normalized;

            float jumpHeight = (jumpDuration * jumpForce) / 2.5f;

            float maxJumpHeight = 5f;
            jumpHeight = Mathf.Clamp(jumpHeight, 0f, maxJumpHeight);

            float maxJumpDistance = 10f;
            float jumpDistance = Mathf.Min(jumpHeight, maxJumpDistance);
            float manaCost = jumpDistance / maxJumpDistance * _wallJumpCostCoeff;

            if (playStats.currentMana >= manaCost)
            {
                Vector3 jumpVelocity = jumpDirection * jumpForce + Vector3.up * jumpHeight;
                playerVelocity += jumpVelocity;
                _controller.Move(playerVelocity * Time.deltaTime);
                playStats.currentMana -= manaCost;
                StartCoroutine(DicreaseJump());

                animator.SetTrigger("WindJump");
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

            float manaCost = jumpDistance / maxJumpDistance * _windJumpCostCoeff;

            if (playStats.currentMana >= manaCost)
            {
                playerVelocity.y = jumpDistance;
                playStats.currentMana -= manaCost;

                animator.SetTrigger("WindJump");
            }
            else Debug.Log("Not enough mana for wind jump");
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


    private void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", true);
        
        bool isMovingForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isMovingBackward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool isMovingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool isMovingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        animator.SetBool("IsMovingForward", isMovingForward);
        animator.SetBool("IsMovingBackward", isMovingBackward);
        animator.SetBool("IsMovingLeft", isMovingLeft);
        animator.SetBool("IsMovingRight", isMovingRight);
        
        float moveSpeed = (isMovingForward || isMovingBackward || isMovingLeft || isMovingRight) ? 1f : 0f;
        animator.SetFloat("Speed", moveSpeed);
    }
    
    private void OnGUI()
    {
        if (windSpellInUse)
        {
            GUI.DrawTexture(new Rect(335, 400, 50 * scaleWindFactor, 50 * scaleWindFactor), windTexture); 
        }
    }
}