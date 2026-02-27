using Assets.Scripts.EventBus.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string sceneName;
    [SerializeField] private int respawnChances;
    private int timesRespawned = 0;

    [Header("Player Settings")]
    public GameObject Player;
    public Rigidbody2D PlayerRB2D;
    //public Rigidbody PlayerRB3D;
    public PlayerHealth PlayerHealth;

    public Vector3 SavePosition;

    public PlayerMovement pm;
    private void Awake()
    {
        //ensure there's only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DiePlayer()
    {   
        if (timesRespawned < respawnChances)
        {
            timesRespawned++;
            Player.transform.position = SavePosition;
            PlayerRB2D.linearVelocity = Vector3.zero;
            PlayerHealth.ResetHealth();
            EventBus<UpdatePlayerUIEvent>.Publish(new UpdatePlayerUIEvent(this));
        }
        if (timesRespawned >= respawnChances)
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }
}
