using UnityEngine;
using System.Collections;

public class BossCombat : EnemyCombat
{
    [Header("Bullet")]
    [SerializeField] private int timesToShootBullet;
    [SerializeField] private float wait;

    protected override void Shoot()
    {
        StartCoroutine(ShootWithDelay());
    }

    private IEnumerator ShootWithDelay()
    {
        if (bullet == null || bulletPos == null) yield break;

        for (int i = 0; i < timesToShootBullet; i++)
        {
            Instantiate(bullet, bulletPos.position, Quaternion.identity);
            //Debug.Log("Boss shoot");
            yield return new WaitForSeconds(wait);
        }
    }
}