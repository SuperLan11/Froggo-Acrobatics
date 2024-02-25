using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int level;
    public void OnClick()
    {
        Frog.selectedLevel = level;
        CommandManager.commandsEnabled = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Real Game");
    }
}
