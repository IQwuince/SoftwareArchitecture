using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public string sceneName;

    private void Update()
    {
        if (enemyHealth.currentHealth <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
