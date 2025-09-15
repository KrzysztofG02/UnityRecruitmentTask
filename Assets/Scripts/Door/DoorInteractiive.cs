using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class DoorInteractive : MonoBehaviour, IInteractive
{
    private GameManager gameManager;
    private UIManager uiManager;
    private Animator animator;
    private AudioSource audioSource;
    private Player player;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string animationBoolParameterName = "IsOpen";

    [Header("Door Sounds")]
    [SerializeField] private AudioClip openSound;

    private void Awake()
    {
        this.gameManager = FindFirstObjectByType<GameManager>().GetComponent<GameManager>();
        this.uiManager = FindFirstObjectByType<UIManager>();
        this.animator = GetComponent<Animator>();
        this.audioSource = GetComponentInChildren<AudioSource>();

        this.audioSource.playOnAwake = false;
        this.audioSource.loop = false;

        GameObject playerObject = GameObject.FindGameObjectWithTag(this.playerTag);
        this.player = playerObject.GetComponent<Player>();
    }

    public void Interact()
    {
        if (this.player.hasKey)
        {
            this.uiManager.ShowOpenPopup( () => Open() );
        }
        else
        {
            this.uiManager.ShowKeyRequiredPopup();
        }
    }

    private void Open()
    {
        if (!this.animator.GetBool(this.animationBoolParameterName))
        {
            this.animator.SetBool(this.animationBoolParameterName, true);
            this.audioSource.PlayOneShot(this.openSound);
            
            this.uiManager.FinishGame();
        }
    }
}
