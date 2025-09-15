using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public AudioClip musicClip;
    private AudioSource audioSource;

    [Header("References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform playerStartTransform;
    private PlayerController playerController;
    private Player player;

    [Header("Spawners")]
    [SerializeField] private GameObject roomManager;
    private DoorSpawner doorSpawner;
    private ChestSpawner chestSpawner;

    [Header("UI Manager")]
    [SerializeField] private UIManager uiManager;

    private bool timerRunning = false;
    public float gameTime { get; private set; } = 0f;
    public float bestGameTime { get; private set; } = float.MaxValue;



    private void Awake()
    {
        if(FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.audioSource = this.GetComponent<AudioSource>();

            this.audioSource.clip = this.musicClip;
            this.audioSource.loop = true;
            this.audioSource.playOnAwake = false;
            this.audioSource.volume = 0.5f;

            this.doorSpawner = this.roomManager.GetComponent<DoorSpawner>();
            this.chestSpawner = this.roomManager.GetComponent<ChestSpawner>();

            this.playerController = this.playerObject.GetComponent<PlayerController>();
            this.player = this.playerObject.GetComponent<Player>();

            DontDestroyOnLoad(this.gameObject);
        }   
    }

    private void Start()
    {
        this.audioSource.Play();
    }

    private void Update()
    {
        if (this.timerRunning)
        {
            this.gameTime += Time.deltaTime;
        }
    }



    public void SetGame()
    {
        this.playerController.SetTransform(playerStartTransform);
        this.player.hasKey = false;

        this.doorSpawner.SpawnDoor();
        this.chestSpawner.SpawnChest();

        this.gameTime = 0f;
        this.timerRunning = true;

        this.uiManager.ToggleTimerVisibility(true);
    }

    public void FinishGame()
    {
        this.timerRunning = false;

        if (this.gameTime < this.bestGameTime)
        {
            this.bestGameTime = this.gameTime;
        }
    }

    private void ClearGame()
    {
        this.uiManager.ClearOverlay();

        DoorInteractive door = FindFirstObjectByType<DoorInteractive>();
        Destroy(door.gameObject);

        ChestInteractive chest = FindFirstObjectByType<ChestInteractive>();
        Destroy(chest.gameObject);
    }

    public void ResetGame()
    {
        this.ClearGame();
        this.SetGame();
    }
}
