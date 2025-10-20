using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class W59_LaneController : MonoBehaviour
{
    [SerializeField] private W59_Map[] mapInstances;
    [SerializeField] private Transform mapContainer;

    private List<W59_Lane> lanes = new();
    private List<W59_Enums.VEHICLE_TYPE> availableTypes;

    public List<W59_Lane> CreateLanes(W59_LevelSO cfg, List<W59_Enums.VEHICLE_TYPE> types)
    {
        availableTypes = types;

        var mapIndex = mapInstances[cfg.totalNumOfLanes - 1];
        var newMap = Instantiate(mapIndex, Vector3.zero, Quaternion.identity, mapContainer);

        foreach (var lane in newMap.lanes)
        {
            SetGateType(lane);
            lanes.Add(lane);
        }

        return lanes;
    }

    private void SetGateType(W59_Lane lane)
    {
        lane.SetType(RandomVehicleType());
    }

    public void SwitchGateTypes()
    {
        foreach (var lane in lanes)
        {
            lane.SetType(RandomVehicleType());
        }
    }

    private W59_Enums.VEHICLE_TYPE RandomVehicleType()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableTypes.Count);

        return availableTypes[randomIndex];
    }

    [Serializable]
    public struct LaneTransforms
    {
        public Transform[] transforms;
    }
}