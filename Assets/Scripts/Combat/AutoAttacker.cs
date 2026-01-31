using System.Collections;
using UnityEngine;

namespace Combat
{
    /** Attacks nearby game objects */
    public class AutoAttacker : MonoBehaviour
    {
        [SerializeField] public string[] targetTags;
        
        [SerializeField] [Range(0f, 10f)] public float attackRange = 3.3f;
        [SerializeField] [Range(0.1f, 10f)] public float attackRate = 2.5f;
        
        [SerializeField] [Range(1, 100)] public int attackDamage = 10;
        [SerializeField] public DamageType attackType;

        public bool CanAttack { get; private set; } = true;

        private void Awake()
        {
            targetTags ??= new string[] { };
        }

        public IEnumerator StartAttackCooldown()
        {
            CanAttack = false;
            yield return new WaitForSeconds(1.0f / attackRate);
            CanAttack = true;
        }

        private void FixedUpdate()
        {
            if (!CanAttack) return;
            
            HealthComponent targetHealth = HealthComponent.GetClosestHealthComponent(
                gameObject,
                attackRange,
                targetTags
            );
            if (!targetHealth) return;

            CanAttack = false;
            targetHealth.Damage(attackDamage, attackType);
            StartCoroutine(StartAttackCooldown());
        }
    }
}
