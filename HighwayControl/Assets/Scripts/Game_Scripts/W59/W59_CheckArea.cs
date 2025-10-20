using UnityEngine;

public class W59_CheckArea : MonoBehaviour
{
    [SerializeField] private W59_Lane lane;
    
    public void OnTriggerStay(Collider obj)
    {
        if (obj.GetComponentInParent<W59_Vehicle>() == null) return;

        lane.collidingVehicle = obj.GetComponentInParent<W59_Vehicle>();
    }

    private void OnTriggerExit(Collider other)
    {
        lane.collidingVehicle = null;
    }

}