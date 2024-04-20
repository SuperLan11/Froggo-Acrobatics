using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneToGameTransition : MonoBehaviour
{
    void Update()
    {
        float progress = (float)GetComponent<VideoPlayer>().time / (float)GetComponent<VideoPlayer>().clip.length;
        Debug.Log(progress);
        if (progress >= 0.99)
        {
            SceneManager.LoadScene("Real Game");
        }
    }
}
