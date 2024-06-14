using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControler : MonoBehaviour
{
    private bool gateState;
    private Vector3 firstPos;
    private Vector3 nextPos;

    [Header("Activated Levers")]
    [SerializeField] private List<GameObject> activatedLevers = new List<GameObject>();

    [Header("Deactivated Levers")]
    [SerializeField] private List<GameObject> deactivatedLevers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        firstPos = GetComponentInChildren<Transform>().position;
        nextPos = new Vector3(firstPos.x, firstPos.y - 11, firstPos.z);
        CheckLeversState();
    }

    // Update is called once per frame
    void Update()
    {
        if (gateState)
        {
            var current = Vector3.MoveTowards(transform.position, nextPos, 5 * Time.deltaTime);
            transform.position = current;

        }
        else
        {
            var current = Vector3.MoveTowards(transform.position, firstPos, 5 * Time.deltaTime);
            transform.position = current;
        }
    }

    public void CheckLeversState()
    {
        bool actvLever = true;
        bool deactvLever = false;

        //Check is all the levers that need to be activated are
        foreach (GameObject lever in activatedLevers)
        {
            if (lever.GetComponent<LeverActions>().GetLeverState() != true)
            {
                actvLever = false;
            }
        }

        //Check is all the levers that need to be deactivated are
        foreach (GameObject lever in deactivatedLevers)
        {
            if (lever.GetComponent<LeverActions>().GetLeverState() != false)
            {
                deactvLever = true;
            }
        }

        if (actvLever && !deactvLever)
        {
            gateState = true;
            gameObject.GetComponent<AudioSource>().Play();
        }
        else
        {
            gateState = false;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
