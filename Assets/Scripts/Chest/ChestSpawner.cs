using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("Chest Settings")]
    [SerializeField] private GameObject chestWithKeyPrefab;
    [SerializeField] private string chestName = "Chest";
    [SerializeField] private string floorTag = "Floor";

    private Transform chestTransform;
    private GameObject floor;
    private float marginFromFloorEdges;



    private void Awake()
    {
        this.chestTransform = this.chestWithKeyPrefab.transform.Find(this.chestName);
        this.floor = GameObject.FindGameObjectWithTag(this.floorTag);

        this.marginFromFloorEdges = this.chestTransform.localScale.x;
    }



    public void SpawnChest()
    {
        Vector3 chestPos = CalculateChestPosition();
        Quaternion chestRot = CalculateChestRotation(chestPos);

        Instantiate(this.chestWithKeyPrefab, chestPos, chestRot);
    }


    private Vector3 CalculateChestPosition()
    {
        Vector3 floorCenter = this.floor.transform.position;
        Vector3 chestPosition = floorCenter;

        float chestLocalY = this.chestTransform != null ? this.chestTransform.localPosition.y : 0f;

        chestPosition.y = floorCenter.y + this.floor.transform.localScale.y / 2f - chestLocalY;

        float halfX = this.floor.transform.localScale.x / 2f;
        float halfZ = this.floor.transform.localScale.z / 2f;

        float randomX = Random.Range(-halfX + this.marginFromFloorEdges, halfX - this.marginFromFloorEdges);
        float randomZ = Random.Range(-halfZ + this.marginFromFloorEdges, halfZ - this.marginFromFloorEdges);

        chestPosition += this.floor.transform.right * randomX + this.floor.transform.forward * randomZ;

        return chestPosition;
    }


    private Quaternion CalculateChestRotation(Vector3 chestPos)
    {
        float floorMinZ = this.floor.transform.position.z - this.floor.transform.localScale.z / 2f;
        float floorMaxZ = this.floor.transform.position.z + this.floor.transform.localScale.z / 2f;

        float distBack = Mathf.Abs(chestPos.z - floorMinZ);
        float distFront = Mathf.Abs(chestPos.z - floorMaxZ);

        float yRotation = distBack < distFront ? 0f : 180f;

        return Quaternion.Euler(0f, yRotation, 0f);
    }
}
