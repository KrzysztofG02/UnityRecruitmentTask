using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ChestInteractive : MonoBehaviour, IInteractive
{
    private Animator animator;
    private AudioSource audioSource;
    private UIManager uiManager;

    [SerializeField] private string animationBoolParameterName = "IsOpen";
    [SerializeField] private string animationFloatParameterName = "Speed";
    [SerializeField] private string animationClipName = "Open";

    [Header("Chest Sounds")]
    public AudioClip openSound;
    public AudioClip closeSound;



    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.audioSource = this.GetComponentInChildren<AudioSource>();
        this.audioSource.playOnAwake = false;
        this.audioSource.loop = false;

        this.uiManager = FindFirstObjectByType<UIManager>();
    }


    public void Interact()
    {
        if (this.animator.GetBool(this.animationBoolParameterName))
        {
            this.Close();
        }
        else
        {
            this.uiManager.ShowOpenPopup(() =>
            {
                this.Open();
            });
        }
    }

    private void Open()
    {
        this.animator.SetBool(this.animationBoolParameterName, true);
        this.animator.SetFloat(this.animationFloatParameterName, 1f);
        this.animator.Play(this.animationClipName, 0, 0f);

        this.audioSource.PlayOneShot(this.openSound);
    }

    private void Close()
    {
        this.animator.SetBool(this.animationBoolParameterName, false);
        this.animator.SetFloat(this.animationFloatParameterName, -1f);
        this.animator.Play(this.animationClipName, 0, 1f);

        this.audioSource.PlayOneShot(this.closeSound);
    }
}
