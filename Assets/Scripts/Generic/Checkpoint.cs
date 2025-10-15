using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //3D
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SavePosition = transform.position;
        }
    }*/


    //2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SavePosition = transform.position;
        }
    }
}

