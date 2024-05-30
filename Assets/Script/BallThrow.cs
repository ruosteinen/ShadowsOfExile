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
            HandleBallSelection();

            if (Input.GetMouseButtonDown(0) && Time.time > lastFireTime + fireRate)
            {
                SpawnAndThrowBall(currentBallPrefab, currentBallSpeed);
                lastFireTime = Time.time;
            }
        }
    }

    private void HandleBallSelection()
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
    }

    private void SpawnAndThrowBall(GameObject ballPrefab, float ballSpeed)
    {
        if (playStats.currentMana < manaCost)
        {
            return;
        }

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Vector3 spawnPoint = playerCamera.transform.position + ray.direction * spawnDistance;

        GameObject ball = Instantiate(ballPrefab, spawnPoint, Quaternion.identity);

        InitializeBall(ball, ballPrefab);

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        if (ballRB != null)
        {
            ballRB.velocity = ray.direction * ballSpeed;
            playStats.currentMana -= manaCost;
        }
    }

    private void InitializeBall(GameObject ball, GameObject ballPrefab)
    {
        if (ballPrefab == fireballPrefab)
        {
            FireBall fireBallScript = ball.GetComponent<FireBall>();
            if (fireBallScript != null)
            {
                fireBallScript.Initialize(playerController.transform.position);
            }
        }
        else if (ballPrefab == waterballPrefab)
        {
            WaterBall waterBallScript = ball.GetComponent<WaterBall>();
            if (waterBallScript != null)
            {
                waterBallScript.Initialize(playerController.transform.position);
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