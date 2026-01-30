using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoxCollider2D meleeCollider;
    [SerializeField] private CapsuleCollider2D playerCollider;
    private PlayerMovement pm;

    [Header("Animations")]
    public Animator attackingLeftAnimator;
    public Animator attackingRightAnimator;

    HashSet<EnemyHealth> IDamageable = new ();
    private float initialOffsetX;

    public int damageAmount;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
    }
    void Start()
    {
        initialOffsetX = meleeCollider ? meleeCollider.transform.localPosition.x : 1f;
    }
    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamageUpgradeEvent>(IncreaseDamage);
    }
    void OnDisable()
    {
        EventBus.UnSubscribe<PlayerDamageUpgradeEvent>(IncreaseDamage);
    }

    private void Update()
    {
        FlipCollider();
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
                DoDamage();
                if (pm.isFlipped == true)
                {
                attackingLeftAnimator.SetTrigger("AttackingLeft");
                }
             if (pm.isFlipped == false)
            {
                attackingRightAnimator.SetTrigger("AttackingRight");
            }
        }
    }

    private void FlipCollider()
    {
        if (meleeCollider == null) return;
        float x = Mathf.Abs(initialOffsetX) * (pm != null && pm.isFlipped ? -1f : 1f);
        meleeCollider.transform.localPosition = new Vector3(x, meleeCollider.transform.localPosition.y, meleeCollider.transform.localPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy") && collider.TryGetComponent<EnemyHealth>(out EnemyHealth eh))
        {
            IDamageable.Add(eh);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy") && collider.TryGetComponent<EnemyHealth>(out EnemyHealth eh))
        {
            IDamageable.Remove(eh);
        }
    }
    
    private void DoDamage()
    {
        foreach (var eh in IDamageable)
        {
          eh.TakeDamage(damageAmount);  
        }
    }
    void IncreaseDamage(PlayerDamageUpgradeEvent playerDamageUpgradeEvent)
    {
        damageAmount += playerDamageUpgradeEvent.damageValue;
    }


}