using UnityEngine;

public class LoadSceneButton : MonoBehaviour
{
    public string scene;
    
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void DisableCommands()
    {
        CommandManager.commandsEnabled = false;
    }
}
