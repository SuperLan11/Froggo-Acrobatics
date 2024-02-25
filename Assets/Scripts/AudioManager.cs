using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource clearStageSource;
    public AudioSource jumpSource;
    public AudioSource tongueShoot;
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
}