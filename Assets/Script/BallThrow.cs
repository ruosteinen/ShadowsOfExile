using UnityEngine;

public class BallThrow : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject waterballPrefab;
    public float fireBallSpeed = 80f;
    public float waterBallSpeed = 80f;
    public float spawnDistance = 1f;
    public Camera playerCamera;
    public PlayerQ3LikeController playerController;
    public int manaCost = 10;
    public float fireRate;
    public float waterRate;
    private float lastFireTime;
    private float lastWaterTime;

    private GameObject currentBallPrefab;
    private float currentBallSpeed;
    
    public Texture2D fireballTexture;
    public Texture2D waterballTexture;
    private float scaleFireFactor = 1.5f;
    private float scaleWaterFactor = 1.5f;
    
    public PlayStats playStats;
    
    void Start()
    {
        currentBallPrefab = fireballPrefab;
        currentBallSpeed = fireBallSpeed;
    }

    private void Update()
    {
        if (playerCamera != null && !PauseMenuSingleton.Instance.IsPaused && playStats.currentMana >= manaCost)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                currentBallPrefab = fireballPrefab;
                currentBallSpeed = fireBallSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentBallPrefab = waterballPrefab;
                currentBallSpeed = waterBallSpeed;
            }

            if (Input.GetMouseButtonDown(0) && Time.time > lastFireTime + fireRate)
            {
                SpawnAndThrowBall(currentBallPrefab, currentBallSpeed);
                lastFireTime = Time.time;
            }
        }
    }

    private void SpawnAndThrowBall(GameObject ballPrefab, float ballSpeed)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Vector3 spawnPoint = ray.origin + ray.direction * spawnDistance;
        Vector3 throwPosition = playerController.transform.position;
        GameObject ball = Instantiate(ballPrefab, spawnPoint, Quaternion.identity);

        if (ballPrefab == fireballPrefab && playStats.currentMana >= manaCost)
        {
            FireBall fireBallScript = ball.GetComponent<FireBall>();
            if (fireBallScript != null) fireBallScript.Initialize(throwPosition);
        }

        if (ballPrefab == waterballPrefab && playStats.currentMana >= manaCost)
        {
            WaterBall waterBallScript = ball.GetComponent<WaterBall>();
            if (waterBallScript != null) waterBallScript.Initialize(throwPosition);
        }

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        if (ballRB != null && playStats.currentMana >= manaCost)
        {
            Vector3 playerVel = playerController.playerVelocity;
            ballRB.velocity = (ray.direction * ballSpeed) + playerVel;
            playStats.currentMana -= manaCost;
        }
    }
    
    void OnGUI()
    {
        if (currentBallPrefab == fireballPrefab)
        {
            GUI.DrawTexture(new Rect(180, 260, 50 * scaleFireFactor, 50 * scaleFireFactor), fireballTexture); 
        }
        else if (currentBallPrefab == waterballPrefab)
        {
            GUI.DrawTexture(new Rect(180, 260, 50 * scaleWaterFactor , 50 * scaleWaterFactor), waterballTexture);
        }
    }
}

