using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public static MainMenuMusic instance;
    
    public void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Stop()
    {
        GetComponent<AudioSource>().Stop();
    }
}
