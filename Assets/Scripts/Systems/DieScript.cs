using UnityEngine;

public class DieScript : MonoBehaviour
{
    public PlayerHealth playerHealth;

    /* //3D
     private void OnCollisionEnter(Collision collision)
     {
         GameManager.Instance.Die();
     }*/

    //2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.DiePlayer();
            Debug.Log("DieScript Collision");
        }
    }
}
