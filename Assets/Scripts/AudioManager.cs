using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource clearStageSource;
    public AudioSource jumpSource;
    public AudioSource tongueShoot;
    public AudioSource spikeDeathSource;
    public AudioSource tongueMetalSource;
    void Awake()
    {
        instance = this;
    }

    public void PlayClearStage()
    {
        clearStageSource.Play();
    }

    public void PlayJump()
    {
        jumpSource.Play();
    }

    public void PlayTongueShoot()
    {
        tongueShoot.Play();
    }

    public void PlayTongueHitMetal()
    {
        tongueMetalSource.Play();
    }
    
    public void PlaySpikeDeath()
    {
        spikeDeathSource.Play();
    }
}