using UnityEngine;

public class BallThrow : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject waterballPrefab;
    public float fireBallSpeed;
    public float waterBallSpeed;
    public float spawnDistance = 1f;
    public Camera playerCamera;
    public PlayerQ3LikeController playerController;

    private void Update()
    {
        if (playerCamera != null && !PauseMenuSingleton.Instance.IsPaused && playerController.mana > 1f)
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                SpawnAndThrowBall(fireballPrefab, fireBallSpeed);
            else if (Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(0))
                SpawnAndThrowBall(waterballPrefab, waterBallSpeed);
        }
    }

    private void SpawnAndThrowBall(GameObject ballPrefab, float ballSpeed)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Vector3 spawnPoint = ray.origin + ray.direction * spawnDistance;
        Vector3 throwPosition = playerController.transform.position;
        GameObject ball = Instantiate(ballPrefab, spawnPoint, Quaternion.identity);

        if (ballPrefab == fireballPrefab)
        {
            FireBall fireBallScript = ball.GetComponent<FireBall>();
            if (fireBallScript != null)
                fireBallScript.Initialize(throwPosition);
        }
        
        if (ballPrefab == waterballPrefab)
        {
            WaterBall waterBallScript = ball.GetComponent<WaterBall>();
            if (waterBallScript != null) waterBallScript.Initialize(throwPosition);
        }

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        if (ballRB != null)
        {
            Vector3 playerVel = playerController.playerVelocity;
            ballRB.velocity = (ray.direction * ballSpeed) + playerVel;
            playerController.mana--;
        }
    }
}