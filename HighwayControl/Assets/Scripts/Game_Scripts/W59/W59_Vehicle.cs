using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class W59_Vehicle : MonoBehaviour
{
    [SerializeField] private GameObject[] vehicleModels;
    [SerializeField] private MeshRenderer[] vehicleMeshRenderers;

    private W59_Enums.VEHICLE_TYPE type;
    private GameObject activeModel;
    private Material activeMaterial;
    private float speed;
    public bool tagScanned;
    private W59_Lane assignedLane;
    private W59_LevelController levelController;

    private void Start()
    {
        levelController = FindObjectOfType<W59_LevelController>();
    }

    public void SetVehicleType(W59_Enums.VEHICLE_TYPE randomType, W59_Lane lane)
    {
        gameObject.SetActive(true);
        tagScanned = false;
        type = randomType;
        assignedLane = lane;

        activeModel = vehicleModels[(int)type];
        activeMaterial = vehicleMeshRenderers[(int)type].material;
        SetVehicleColor();

        foreach (var model in vehicleModels)
        {
            model.SetActive(false);
        }

        activeModel.SetActive(true);
    }

    public W59_Enums.VEHICLE_TYPE GetType()
    {
        return type;
    }

    public void Move(W59_Lane lane, float speed)
    {
        transform.position = lane.laneStart.position;
        transform.DOMove(lane.laneGatePassPoint.position, speed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GatePassed();
                
                transform.DOMove(lane.laneEnd.position, speed)
                    .SetSpeedBased()
                    .SetEase(Ease.Linear)
                    .OnComplete(RemoveVehicle);
            });
    }

    private void GatePassed()
    {
        if (!tagScanned)
        {
            levelController.OnVehiclePass(assignedLane.acceptedType == type);
        }
    }

    private void SetVehicleColor()
    {
        float randomRed = Random.Range(0f, 1f);
        float randomGreen = Random.Range(0f, 1f);
        float randomBlue = Random.Range(0f, 1f);

        var randomColor = new Color(randomRed, randomGreen, randomBlue);

        activeMaterial.color = randomColor;
    }

    public void RemoveVehicle()
    {
        gameObject.SetActive(false);
    }

    public void SetVehicleAsScanned()
    {
        tagScanned = true;
    }
}