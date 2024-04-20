using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource clearStageSource;
    public AudioSource jumpSource;
    public AudioSource tongueShoot;
    public AudioSource checkpointCollectSource;
    public AudioSource deathSource;
    public AudioSource tongueHitSource;
    public AudioSource leverActivateSource;
    public AudioSource littleJumpSource;
    public AudioSource crashSource;
    public AudioSource cutsceneMusicSource;
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

    public void PlayDeath()
    {
        deathSource.Play();
    }
    
    public void PlayCollectCheckpoint()
    {
        checkpointCollectSource.Play();
    }

    public void PlayTongueHit()
    {
        tongueHitSource.Play();
    }

    public void PlayLeverActivate()
    {
        leverActivateSource.Play();
    }

    public void PlayLittleJump()
    {
        littleJumpSource.Play();
    }

    public void PlayCrash()
    {
        crashSource.Play();
    }
}