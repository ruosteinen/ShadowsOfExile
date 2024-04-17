using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SearchTest : MonoBehaviour
{
    public float rotateSpeed = 2f;
    private Vector3[] rotationDirection =
    {
        Vector3.right,
        Vector3.left,
        Vector3.back,
        Vector3.forward
    };

    private Vector3 newDirection;

    private int num = 0;
    private bool rotationComplete = false;

    private void Start()
    {
        rotationDirection[0] = transform.right;
        rotationDirection[1] = -transform.right;
        rotationDirection[2] = -transform.forward;
        rotationDirection[3] = transform.forward;
    }

    void Update()
    {
        float singleStep = rotateSpeed * Time.deltaTime;

        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[num])) <= 2)
        {
            if (num < rotationDirection.Length - 1)
            {
                num++;
            }
            else
            {
                rotationComplete = true;
                Debug.Log(rotationComplete);
            }
        }
        else
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rotationDirection[num])) >= 2 && num < rotationDirection.Length)
            {
                newDirection = Vector3.RotateTowards(transform.forward, rotationDirection[num], singleStep, 0.0f);

                transform.rotation = Quaternion.LookRotation(newDirection);
            }                
        }
    }
}