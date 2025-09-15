using UnityEngine;

public class KeyInteractive : MonoBehaviour, IInteractive
{
    [Header("Key Sounds")]
    [SerializeField] private AudioClip pickUpSound;

    private Player player;
    private UIManager uiManager;



    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        this.player = playerObject.GetComponent<Player>();

        this.uiManager = FindFirstObjectByType<UIManager>();
    }

    public void Interact()
    {
        if (this.uiManager != null)
        {
            this.uiManager.ShowTakePopup(() =>
            {
                PickUp();
            });
        }
    }

    private void PickUp()
    {
        this.player.PickUpKey();

        AudioSource.PlayClipAtPoint(this.pickUpSound, this.transform.position);

        Destroy(this.gameObject);
    }
}
