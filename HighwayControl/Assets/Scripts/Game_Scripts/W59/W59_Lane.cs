using System.Collections;
using UnityEngine;

public class W59_Lane : MonoBehaviour
{
    public W59_Enums.VEHICLE_TYPE acceptedType;
    public W59_Vehicle collidingVehicle;

    public Transform laneStart;
    public Transform laneGatePassPoint;
    public Transform laneEnd;

    public GameObject[] billboardContainers;
    public GameObject flashExplosion;
    public Camera ticketCamera;

    public void SetType(W59_Enums.VEHICLE_TYPE type)
    {
        foreach (var billboard in billboardContainers)
        {
            billboard.SetActive(false);
        }

        acceptedType = type;
        billboardContainers[(int)acceptedType].SetActive(true);
        Taptic.Warning();
    }

    public void Explode()
    {
        flashExplosion.SetActive(false);
        flashExplosion.SetActive(true);

        StartCoroutine(TakePhoto());
    }

    private IEnumerator TakePhoto()
    {
        ticketCamera.enabled = true;
        yield return new WaitForEndOfFrame();
        ticketCamera.enabled = false;
    }
}