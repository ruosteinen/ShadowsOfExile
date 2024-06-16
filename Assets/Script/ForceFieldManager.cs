using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldManager : MonoBehaviour
{
    public GameObject forceField; // Reference to the force field GameObject
    public List<GameObject> enemies; // List of enemy GameObjects

    void Start()
    {
        // Optionally, initialize the list if it's empty
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogWarning("Enemy list is empty. Please add enemies to the list.");
        }
    }

    void Update()
    {
        CheckEnemies();
    }

    void CheckEnemies()
    {
        // Remove any null entries from the list (i.e., destroyed enemies)
        enemies.RemoveAll(enemy => enemy == null);

        // Check if the list is empty
        if (enemies.Count == 0)
        {
            DestroyForceField();
        }
    }

    void DestroyForceField()
    {
        if (forceField != null)
        {
            Destroy(forceField);
            Debug.Log("Force field destroyed!");
        }
        else
        {
            Debug.LogWarning("Force field reference is not set.");
        }
    }
}