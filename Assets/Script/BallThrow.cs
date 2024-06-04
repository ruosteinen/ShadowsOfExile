using UnityEngine;

public class BallThrow : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject waterballPrefab;
    public float ballSpeed = 80f;
    public Transform spawnTransform;
    public Camera playerCamera;
    public int manaCost = 10;
    public float fireRate = 0.5f;
    private float lastFireTime;

    private GameObject currentBallPrefab;

    public Texture2D fireballTexture;
    public Texture2D waterballTexture;
    private float scaleFireFactor = 1.5f;
    private float scaleWaterFactor = 1.5f;

    public PlayStats playStats;
    public LayerMask aimColliderLayerMask;
    public Transform debugTransform;

    private Animator animator;

    void Start()
    {
        currentBallPrefab = fireballPrefab;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!PauseMenuSingleton.Instance.IsPaused && playStats.currentMana >= manaCost)
        {
            HandleBallSelection();

            if (Input.GetButtonDown("Fire1") && Time.time > lastFireTime + fireRate)
            {
                Shoot();
                lastFireTime = Time.time;
            }
        }
    }

    private void HandleBallSelection()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentBallPrefab = fireballPrefab;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentBallPrefab = waterballPrefab;
        }
    }

    private void Shoot()
    {
        if (playStats.currentMana < manaCost) return;

        GameObject ball = Instantiate(currentBallPrefab, spawnTransform.position, spawnTransform.rotation);
        Rigidbody ballRB = ball.GetComponent<Rigidbody>();

        InitializeBall(ball);

        if (ballRB != null)
        {
            if (currentBallPrefab == fireballPrefab)
            {
                animator.SetTrigger("ThrowFireball");
            }
            else if (currentBallPrefab == waterballPrefab)
            {
                animator.SetTrigger("ThrowWaterball");
            }
            
            Vector3 shootDirection = CalculateShootDirection();

            ballRB.AddForce(shootDirection * ballSpeed, ForceMode.Impulse);
            playStats.currentMana -= manaCost;
        }
    }

    private Vector3 CalculateShootDirection()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenterPoint);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            targetPoint = raycastHit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }

        Vector3 shootDirection = (targetPoint - spawnTransform.position).normalized;
        return shootDirection;
    }

    private void InitializeBall(GameObject ball)
    {
        if (currentBallPrefab == fireballPrefab)
        {
            FireBall fireBallScript = ball.GetComponent<FireBall>();
            if (fireBallScript != null)
            {
                fireBallScript.Initialize(transform.position);
            }
        }
        else if (currentBallPrefab == waterballPrefab)
        {
            WaterBall waterBallScript = ball.GetComponent<WaterBall>();
            if (waterBallScript != null)
            {
                waterBallScript.Initialize(transform.position);
            }
        }
    }

    void OnGUI()
    {
        if (currentBallPrefab == fireballPrefab)
        {
            GUI.DrawTexture(new Rect(180, 400, 50 * scaleFireFactor, 50 * scaleFireFactor), fireballTexture);
        }
        else if (currentBallPrefab == waterballPrefab)
        {
            GUI.DrawTexture(new Rect(180, 400, 50 * scaleWaterFactor, 50 * scaleWaterFactor), waterballTexture);
        }
    }
}