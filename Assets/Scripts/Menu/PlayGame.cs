using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    public string sceneName;
    public void Play()
    {
        SceneManager.LoadScene(sceneName);
    }
}
