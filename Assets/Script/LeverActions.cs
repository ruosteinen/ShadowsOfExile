using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverActions : MonoBehaviour
{
    [SerializeField] Transform playerCam;
    [SerializeField] GateControler[] gateControler;

    [SerializeField] private GameObject leverOn;
    [SerializeField] private GameObject leverOff;

    private bool leverState;
    private bool active;

    private float playerActivateDistance = 10f;

    // Static variable to track global gate state
    private static bool globalGateState;

    void Update()
    {
        RaycastHit hit;
        active = Physics.Raycast(playerCam.position, playerCam.TransformDirection(Vector3.forward), out hit, playerActivateDistance);
        if (active && hit.collider.gameObject == gameObject && Input.GetKeyDown(KeyCode.E))
        {
            leverState = !leverState;
            CallGateChecks();

            // gameObject.GetComponent<AudioSource>().Play();
            leverOn.SetActive(leverState);
            leverOff.SetActive(!leverState);
            // Debug.Log(leverState);
        }

        // Check for global gate toggle
        if (Input.GetKeyDown(KeyCode.L))
        {
            globalGateState = !globalGateState;
            ToggleAllGates(globalGateState);
        }
    }

    public bool GetLeverState()
    {
        return leverState;
    }

    private void CallGateChecks()
    {
        for (int i = 0; i < gateControler.Length; i++)
        {
            gateControler[i].CheckLeversState();
        }
    }

    private void ToggleAllGates(bool state)
    {
        foreach (var gate in gateControler)
        {
            gate.SetGateState(state);
        }
    }
}
