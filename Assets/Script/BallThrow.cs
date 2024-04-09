/*using UnityEngine;

public class BallThrow : MonoBehaviour
{
    public GameObject currentBall;
    public GameObject fireballPrefab;
    public GameObject waterballPrefab;
    public float ballSpeed; 
    //public float waterBallSpeed; 
    public float spawnDistance = 2f;
    public Camera playerCamera;
    public PlayerQ3LikeController playerController;
    private bool fireballThrown = false;
    private bool waterballThrown = false;

    private void Update()
    {
        if (playerCamera != null && !PauseMenuSingleton.Instance.IsPaused && playerController.mana >= 1f)
        {
            if (Input.GetMouseButtonDown(0) && !fireballThrown)
            {
                currentBall = fireballPrefab;
                fireballThrown = true;
                waterballThrown = false;
                ballSpeed = ballSpeed;
            }
            else if (Input.GetMouseButtonDown(1) && !waterballThrown)
            {
                currentBall = waterballPrefab;
                waterballThrown = true;
                fireballThrown = false;
                ballSpeed = ballSpeed;
            }

            if (currentBall != null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                Ray ray = playerCamera.ScreenPointToRay(screenCenter);
                Vector3 spawnPoint = ray.origin + ray.direction * spawnDistance;
                GameObject cBall = Instantiate(currentBall, spawnPoint, Quaternion.identity);
                Rigidbody cBallRB = cBall.GetComponent<Rigidbody>();

                if (cBallRB != null)
                {
                    Vector3 playerVel = playerController.playerVelocity;
                    cBallRB.velocity = (ray.direction * ballSpeed) + playerVel;
                    playerController.mana--;
                }
            }
        }
    }
}*/

using UnityEngine;

public class BallThrow : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float fireBallSpeed;
    public float spawnDistance = 1f;
    public Camera playerCamera;
    public PlayerQ3LikeController playerController;

    private void Update()
    {
        if ( Input.GetMouseButtonDown(0) && playerController.mana > 1f)
        {
            if (playerCamera != null && !PauseMenuSingleton.Instance.IsPaused)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                Ray ray = playerCamera.ScreenPointToRay(screenCenter);
                Vector3 spawnPoint = ray.origin + ray.direction * spawnDistance;
                GameObject fireBall = Instantiate(fireballPrefab, spawnPoint, Quaternion.identity);

                Rigidbody fireBallRB = fireBall.GetComponent<Rigidbody>();
                if (fireBallRB != null)
                {
                    Vector3 playerVel = playerController.playerVelocity;
                    fireBallRB.velocity = (ray.direction * fireBallSpeed) + playerVel;
                    playerController.mana--;
                }
            }
        }
    }
}   
