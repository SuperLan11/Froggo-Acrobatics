using UnityEngine;

public class LoadSceneButton : MonoBehaviour
{
    public string scene;
    public int sceneIndex;
    
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
    
    public void LoadSceneIndex()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void DisableCommands()
    {
        CommandManager.commandsEnabled = false;
    }
}
