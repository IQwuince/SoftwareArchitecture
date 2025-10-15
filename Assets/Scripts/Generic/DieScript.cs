using UnityEngine;

public class DieScript : MonoBehaviour
{

   /* //3D
    private void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.Die();
    }*/

    //2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.Die();
    }
}
