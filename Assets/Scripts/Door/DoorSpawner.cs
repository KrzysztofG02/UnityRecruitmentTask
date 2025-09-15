using System;
using UnityEngine;



public class DoorSpawner : MonoBehaviour
{
    enum Side {Left, Front, Right, Back};

    [Header("Door Settings")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private string floorTag = "Floor";

    private GameObject floor;
    private float marginFromCorners;



    private void Awake()
    {
        this.floor = GameObject.FindGameObjectWithTag(this.floorTag);
        this.marginFromCorners = this.doorPrefab.transform.localScale.z / 2f;
    }



    public void SpawnDoor()
    {
        Side randomSide = (Side)UnityEngine.Random.Range(0, 4);

        Instantiate(
            this.doorPrefab, 
            this.CalculateDoorPosition(floor, randomSide), 
            this.CalculateDoorRotation(floor, randomSide));
    }


    private Vector3 CalculateDoorPosition(GameObject floor, Side side)
    {
        Vector3 floorCenter = floor.transform.position;
        Vector3 doorPosition = floorCenter;

        doorPosition.y = floorCenter.y + floor.transform.localScale.y / 2f;

        SetRandomDoorPosition(floor, side, ref doorPosition);

        return doorPosition;
    }

    private Quaternion CalculateDoorRotation(GameObject floor, Side side)
    {
        return floor.transform.rotation * Quaternion.Euler(0f, 90f * (float)side, 0f);
    }


    private void SetRandomDoorPosition(GameObject floor, Side side, ref Vector3 doorPosition)
    {
        Vector3 right = floor.transform.right;
        Vector3 forward = floor.transform.forward;
        float halfFloorLengthX = floor.transform.localScale.x / 2f;
        float halfFloorLengthZ = floor.transform.localScale.z / 2f;

        switch (side)
        {
            case Side.Left:
            {
                doorPosition += -right * halfFloorLengthX;
                doorPosition +=
                    forward * UnityEngine.Random.Range(-halfFloorLengthZ + this.marginFromCorners, halfFloorLengthZ - this.marginFromCorners);
                return;
            }

            case Side.Front:
            {
                doorPosition += forward * halfFloorLengthZ;
                doorPosition +=
                    right * UnityEngine.Random.Range(-halfFloorLengthX + this.marginFromCorners, halfFloorLengthX - this.marginFromCorners);
                return;
            }

            case Side.Right:
            {
                doorPosition += right * halfFloorLengthX;
                doorPosition +=
                    forward * UnityEngine.Random.Range(-halfFloorLengthZ + this.marginFromCorners, halfFloorLengthZ - this.marginFromCorners);
                return;
            }

            case Side.Back:
            {
                doorPosition += -forward * halfFloorLengthZ;
                doorPosition +=
                    right * UnityEngine.Random.Range(-halfFloorLengthX + this.marginFromCorners, halfFloorLengthX - this.marginFromCorners);
                return;
            }

            default: return;
        }
    }
}