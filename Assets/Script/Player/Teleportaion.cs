using UnityEngine;

public class Teleportation : MonoBehaviour
{
    // Define waypoints as GameObjects in the Unity editor
    public GameObject townWaypoint;
    public GameObject forestWaypoint;
    public GameObject mountainWaypoint;

    // Function to teleport the player to a specified waypoint
    void TeleportTo(GameObject waypoint)
    {
        if (waypoint != null)
        {
            // Move the player to the waypoint's position
            transform.position = waypoint.transform.position;
            Debug.Log("Teleported to " + waypoint.name);
        }
        else
        {
            Debug.Log("Invalid waypoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input to teleport
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
    }
}
//needed to be inside the giant player controller code
