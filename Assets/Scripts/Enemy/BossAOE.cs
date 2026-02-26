using UnityEngine;
using System.Collections;
using System;


public class BossAOE : MonoBehaviour
{
    [Header("AOE")]
    [SerializeField] private float aoeCountDown;
    [SerializeField] private float aoeTime;
    [SerializeField] private float aoeRechargeTime;
    [SerializeField] private float attackInverval;
    public int aoeDamage;
    public GameObject aoeCircle;
    private SpriteRenderer attackCirle;
    public bool isAoeAttacking;
    private bool playerInTrigger = false;
    private Coroutine damageCoroutine;


    private void Start()
    {
        attackCirle = aoeCircle.GetComponent<SpriteRenderer>();
        StartCoroutine(AoeAttackLoop());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (isAoeAttacking && damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamagePlayerPeriodically());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator AoeAttackLoop()
    {
        while (true)
        {
            // Preparation phase
            aoeCircle.SetActive(true);
            attackCirle.color = Color.white;
            yield return new WaitForSeconds(aoeCountDown);

            // Attack phase
            attackCirle.color = Color.red;
            isAoeAttacking = true;
            yield return new WaitForSeconds(aoeTime);

            // End attack
            attackCirle.color = Color.white;
            isAoeAttacking = false;
            aoeCircle.SetActive(false); 
            yield return new WaitForSeconds(aoeRechargeTime);
        }
    }


    private IEnumerator DamagePlayerPeriodically()
    {
        while (playerInTrigger && isAoeAttacking)
        {
            //Debug.Log("AOE Damage");
            EventBus<PlayerDamagedEvent>.Publish(new PlayerDamagedEvent(aoeDamage));
            yield return new WaitForSeconds(attackInverval);
        }
        damageCoroutine = null;
    }

    private void Update()
    {
        // If the player is in the trigger and the boss starts attacking, start the coroutine
        if (playerInTrigger && isAoeAttacking && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamagePlayerPeriodically());
        }
        // If the player is in the trigger but the boss stops attacking, stop the coroutine
        if ((!isAoeAttacking || !playerInTrigger) && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
}
