using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControler : MonoBehaviour
{
    private bool gateState;
    private Vector3 firstPos;
    private Vector3 nextPos;
    public Animator gateAnimator;

    [Header("Activated Levers")]
    [SerializeField] private List<GameObject> activatedLevers = new List<GameObject>();

    [Header("Deactivated Levers")]
    [SerializeField] private List<GameObject> deactivatedLevers = new List<GameObject>();

    private float gateMoveSpeed = 5f;

    void Start()
    {
        firstPos = transform.position;
        nextPos = new Vector3(firstPos.x, firstPos.y - 11, firstPos.z);
        CheckLeversState();
    }

    void Update()
    {
        Vector3 targetPosition = gateState ? nextPos : firstPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, gateMoveSpeed * Time.deltaTime);
    }

    public void CheckLeversState()
    {
        bool actvLever = true;
        bool deactvLever = false;

        foreach (GameObject lever in activatedLevers)
        {
            LeverActions leverActions = lever.GetComponent<LeverActions>();
            gateAnimator.SetBool("isOn", true);
            if (leverActions == null || !leverActions.GetLeverState())
            {
                actvLever = false;
                break;
            }
        }

        foreach (GameObject lever in deactivatedLevers)
        {
            LeverActions leverActions = lever.GetComponent<LeverActions>();
            gateAnimator.SetBool("isOff", true);
            if (leverActions == null || leverActions.GetLeverState())
            {
                deactvLever = true;
                break;
            }
        }

        gateState = actvLever && !deactvLever;
    }
}
