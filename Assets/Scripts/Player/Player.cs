using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hasKey = false;

    public void PickUpKey()
    {
        hasKey = true;
    }
}