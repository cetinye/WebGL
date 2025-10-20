using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class W59_TrafficController : MonoBehaviour
{
    [SerializeField] private W59_GameManager gameManager;

    public List<W59_Vehicle> vehiclePool;
    [SerializeField] private W59_Vehicle vehicleInstance;
    [SerializeField] private Transform vehicleContainer;

    private float spawnDelay;
    private float carSpeed;
    private List<W59_Enums.VEHICLE_TYPE> availableTypes;
    private List<W59_Lane> lanes;

    private W59_Lane lastSpawnLane;

    private void Start()
    {
        CreateVehiclePool();
    }

    public void StartTraffic(W59_LevelSO level, List<W59_Enums.VEHICLE_TYPE> types, List<W59_Lane> spawnedLanes)
    {
        availableTypes = types;
        lanes = spawnedLanes;

        spawnDelay = level.carSpawnDelay;
        carSpeed = level.vehicleSpeed;

        StartCoroutine(SpawnCarRoutine());
    }

    private IEnumerator SpawnCarRoutine()
    {
        while (true)
        {
            var vehicle = GetVehicleFromPool();
            var randomLane = lanes[Random.Range(0, lanes.Count)];

            var delay = spawnDelay;

            if (randomLane == lastSpawnLane)
            {
                delay += 2f;
            }
            lastSpawnLane = randomLane;

            yield return new WaitForSeconds(delay);

            //if(Random.Range(0, 100) < 20) continue;

            //vehicle.SetVehicleType(Random.Range(0, 100) < 90 ? RandomVehicleType() : randomLane.acceptedType, randomLane);
            //vehicle.Move(randomLane, carSpeed);
            //gameManager.playFxBySoundState(W59_Enums.eW59FxSoundStates.VEHICLE, 0.05f);

            int r = Random.Range(0, 100);

            if (r <= 80)
                vehicle.SetVehicleType(GetWrongVehicleType(randomLane), randomLane);

            else
                vehicle.SetVehicleType(GetCorrectVehicleType(randomLane), randomLane);

            vehicle.Move(randomLane, W59_LevelController.LevelSO.vehicleSpeed);
            gameManager.playFxBySoundState(W59_Enums.eW59FxSoundStates.VEHICLE, 0.05f);
        }
    }

    private void CreateVehiclePool()
    {
        for (int i = 0; i < 20; i++)
        {
            W59_Vehicle vehicle = Instantiate(vehicleInstance, Vector3.zero, Quaternion.identity, vehicleContainer);
            vehicle.gameObject.SetActive(false);
            vehicle.transform.position = new Vector3(-1000, 0 - 1000);
            vehiclePool.Add(vehicle);
        }
    }

    W59_Vehicle GetVehicleFromPool()
    {
        foreach (W59_Vehicle obj in vehiclePool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        W59_Vehicle newObj = Instantiate(vehicleInstance, Vector3.zero, Quaternion.identity, vehicleContainer);
        newObj.gameObject.SetActive(false);
        vehiclePool.Add(newObj);

        return newObj;
    }

    private W59_Enums.VEHICLE_TYPE RandomVehicleType()
    {
        int randomIndex = Random.Range(0, availableTypes.Count);

        return availableTypes[randomIndex];
    }

    private W59_Enums.VEHICLE_TYPE GetWrongVehicleType(W59_Lane lane)
    {
        int randomIndex;
        W59_Enums.VEHICLE_TYPE vehicleType;

        do
        {
            randomIndex = Random.Range(0, availableTypes.Count);
            vehicleType = availableTypes[randomIndex];

        } while (vehicleType == lane.acceptedType);

        return vehicleType;
    }

    private W59_Enums.VEHICLE_TYPE GetCorrectVehicleType(W59_Lane lane)
    {
        int randomIndex;
        W59_Enums.VEHICLE_TYPE vehicleType;

        do
        {
            randomIndex = Random.Range(0, availableTypes.Count);
            vehicleType = availableTypes[randomIndex];

        } while (vehicleType != lane.acceptedType);

        return vehicleType;
    }
}