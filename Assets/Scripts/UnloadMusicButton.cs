using UnityEngine;

public class UnloadMusicButton : MonoBehaviour
{
    public void UnloadMusic()
    {
        MainMenuMusic.instance.Stop();
    }
}
