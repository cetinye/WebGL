using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

namespace Chroma_City_Neon_Racing
{
    public class RoadGenerator : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        [Header("Components")]
        public SplineComputer splineComputerRoad;
        public SplineComputer splineComputerBuildings;
        public SplineComputer splineComputerCheckpoints;
        public SplineComputer splineComputerPowerUps;
        public GameObject streetLights;
        public SplineFollower splineFollower;

        [Header("Variables")]
        public int pointAmount;
        private int pointAmountToRandomize;
        public float minX;
        public float maxX;
        public float distBetweenRoads;
        private List<int> randomizedPoints = new List<int>();

        [Header("Building Variables")]
        [SerializeField] private float buildingOffsetToRoad;

        public int passedPointCount = 0;

        public void SpawnLevel()
        {
            passedPointCount = 0;

            SetComputerStates(true);
            SpawnPoints(pointAmount);
            pointAmountToRandomize = Mathf.CeilToInt(pointAmount / 2);
            RandomizePointsOnX(minX, maxX, pointAmountToRandomize);
            CreateBuildingsSpline();
            CreateCheckpointsSpline();
            CreatePowerUpsSpline();
            levelManager.DisableCheckpointOverlaps();
            Invoke(nameof(RemoveExcessObjects), 1f);
            Invoke(nameof(DisableComputers), 3f);
        }

        public void Reset()
        {
            SplinePoint[] points = new SplinePoint[0];

            splineComputerRoad.SetPoints(points);
            splineComputerBuildings.SetPoints(points);
            splineComputerCheckpoints.SetPoints(points);
            splineComputerPowerUps.SetPoints(points);

            randomizedPoints.Clear();

            for (int i = 0; i < splineComputerPowerUps.transform.childCount; i++)
            {
                splineComputerPowerUps.transform.GetChild(i).TryGetComponent<PowerUps>(out PowerUps powerUps);
                powerUps.Reset();
            }
        }

        void SpawnPoints(int pointAmount)
        {
            SplinePoint[] points = new SplinePoint[pointAmount];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new SplinePoint();
                points[i].position = Vector3.forward * (-i) * distBetweenRoads;
                points[i].normal = Vector3.up;
                points[i].size = 1f;
                points[i].color = Color.white;
            }

            splineComputerRoad.SetPoints(points);
            splineFollower.spline = splineComputerRoad;
        }

        void AddTriggers()
        {
            for (int i = 0; i < splineComputerRoad.pointCount; i++)
            {
                double normalizedPosition = splineComputerRoad.GetPointPercent(i);
                splineComputerRoad.AddTrigger(0, normalizedPosition, SplineTrigger.Type.Forward);
                splineComputerRoad.triggerGroups[0].triggers[i].workOnce = true;
                splineComputerRoad.triggerGroups[0].triggers[i].AddListener(OnTriggerCrossed);
            }
        }

        public void OnTriggerCrossed()
        {
            passedPointCount++;
            Debug.Log("OnTriggerCrossed " + passedPointCount);
        }

        void RandomizePointsOnX(float minX, float maxX, int pointAmountToRandomize)
        {
            SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
            points = splineComputerRoad.GetPoints();

            for (int i = 0; i < pointAmountToRandomize; i++)
            {
                int index = GetRandomPointIdxToRandomize();
                points[index].position = new Vector3(UnityEngine.Random.Range(minX, maxX), points[index].position.y, points[index].position.z);
            }

            splineComputerRoad.SetPoints(points);
        }

        int GetRandomPointIdxToRandomize()
        {
            int index;
            SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
            points = splineComputerRoad.GetPoints();

            do
            {
                index = UnityEngine.Random.Range(2, points.Length - 2);

            } while (randomizedPoints.Contains(index));

            randomizedPoints.Add(index);
            return index;
        }

        void CreateBuildingsSpline()
        {
            SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
            points = splineComputerRoad.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                points[i].position = new Vector3(points[i].position.x + buildingOffsetToRoad, -0.11f, points[i].position.z);
            }

            splineComputerBuildings.SetPoints(points);
        }

        void CreateCheckpointsSpline()
        {
            SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
            points = splineComputerRoad.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                points[i].position = new Vector3(points[i].position.x, points[i].position.y, points[i].position.z);
            }

            splineComputerCheckpoints.SetPoints(points);
        }

        void CreatePowerUpsSpline()
        {
            SplinePoint[] points = new SplinePoint[splineComputerRoad.pointCount];
            points = splineComputerRoad.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                points[i].position = new Vector3(points[i].position.x, points[i].position.y, points[i].position.z);
            }

            splineComputerPowerUps.SetPoints(points);
        }

        void RemoveExcessObjects()
        {
            // RemoveExcessObjectsInSpline(splineComputerRoad);
            RemoveExcessObjectsInSpline(splineComputerBuildings);
            RemoveExcessObjectsInSpline(splineComputerPowerUps);
            RemoveExcessObjectsInSpline(splineComputerCheckpoints);

            levelManager.SpawnFinish();
            ExtendRoad();
            AddTriggers();
            RemoveObjectsAfter();
        }

        private void RemoveExcessObjectsInSpline(SplineComputer splineComputer)
        {
            if (splineComputer == null || splineComputer.transform == null)
            {
                return;
            }

            for (int i = 0; i < splineComputer.transform.childCount - 1; i++)
            {
                var child = splineComputer.transform.GetChild(i);
                var nextChild = splineComputer.transform.GetChild(i + 1);

                if (nextChild == null)
                {
                    break;
                }

                if (child.position == nextChild.position)
                {
                    child.gameObject.SetActive(false);
                    nextChild.gameObject.SetActive(false);
                }
            }
        }

        private void ExtendRoad()
        {
            float extensionDistance = -5f;
            Vector3 newPos = new Vector3(splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 1).x, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 1).y, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 1).z + extensionDistance);
            splineComputerRoad.SetPointPosition(splineComputerRoad.pointCount - 1, newPos);

            AddPoint(splineComputerRoad);
            newPos = new Vector3(splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).x, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).y, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).z + extensionDistance);
            splineComputerRoad.SetPointPosition(splineComputerRoad.pointCount - 1, newPos);

            AddPoint(splineComputerRoad);
            newPos = new Vector3(splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).x + extensionDistance, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).y, splineComputerRoad.GetPointPosition(splineComputerRoad.pointCount - 2).z);
            splineComputerRoad.SetPointPosition(splineComputerRoad.pointCount - 1, newPos);

            CreateBuildingsSpline();
            CreatePowerUpsSpline();
            StartCoroutine(RemoveExcessRoutine(splineComputerBuildings));
            StartCoroutine(RemoveExcessRoutine(splineComputerPowerUps));
            Invoke(nameof(RemoveObjectsAfter), 1f);
            // Invoke(nameof(MarkObjectsStatic), 2f);
        }

        private void AddPoint(SplineComputer splineComputer)
        {
            SplinePoint[] orgPoints = splineComputer.GetPoints();
            SplinePoint[] tempPoints = null;
            tempPoints = new SplinePoint[orgPoints.Length + 1];
            System.Array.Copy(orgPoints, 0, tempPoints, 0, orgPoints.Length);
            tempPoints[^1] = new SplinePoint(splineComputer.GetPointPosition(splineComputer.pointCount - 3));
            splineComputer.SetPoints(tempPoints);
        }

        private void RemoveObjectsAfter()
        {
            Vector3 finishLinePosition = levelManager.GetFinishPosition();

            for (int i = 0; i < splineComputerPowerUps.transform.childCount; i++)
            {
                Transform child = splineComputerPowerUps.transform.GetChild(i);
                if (child.position.z < finishLinePosition.z)
                {
                    child.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < splineComputerCheckpoints.transform.childCount; i++)
            {
                Transform child = splineComputerCheckpoints.transform.GetChild(i);
                if (child.position.z < finishLinePosition.z)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void SetPathLength(int newPathLength)
        {
            pointAmount = newPathLength + 2;
        }

        public Vector3 GetRandomPointPos()
        {
            return splineComputerRoad.GetPointPosition(UnityEngine.Random.Range(2, splineComputerRoad.pointCount - 2));
        }

        public void SetComputerStates(bool state)
        {
            splineComputerRoad.enabled = state;
            splineComputerBuildings.enabled = state;
            splineComputerCheckpoints.enabled = state;
            splineComputerPowerUps.enabled = state;
        }

        public void DisableComputers()
        {
            SetComputerStates(false);
        }

        IEnumerator RemoveExcessRoutine(SplineComputer splineComputer)
        {
            yield return new WaitForSeconds(1f);
            RemoveExcessObjectsInSpline(splineComputer);
        }
    }
}